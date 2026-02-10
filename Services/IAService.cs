using System;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Linq;
using ProyectoIntegradorS6G7.Models;

namespace ProyectoIntegradorS6G7.Services
{
    public class IAService
    {
        private readonly AppDbContext _contexto;
        private readonly string _pythonPath;
        private readonly string _scriptPath;

        public IAService(AppDbContext contexto)
        {
            _contexto = contexto;
            _pythonPath = "python"; // o la ruta completa a python.exe
            _scriptPath = Path.Combine(Directory.GetCurrentDirectory(), "prediccion.py");
        }

        /// <summary>
        /// Obtiene la recomendación de IA para un cliente
        /// </summary>
        public RecomendacionIA ObtenerRecomendacion(string ruc)
        {
            var config = _contexto.ConfiguracionIA.FirstOrDefault() ?? new ConfiguracionIA();
            var cliente = _contexto.Clientes.FirstOrDefault(c => c.ruc == ruc);

            if (cliente == null)
            {
                return new RecomendacionIA
                {
                    NivelRiesgo = "ALTO",
                    InteresRecomendado = config.interesRiesgoAlto,
                    CuotasRecomendadas = config.cuotasMinimasPorDefecto,
                    Razonamiento = "Cliente no encontrado",
                    UsaModelo = false
                };
            }

            // Obtener historial
            var creditosAnteriores = _contexto.Creditos
                .Where(c => c.rucCliente == ruc)
                .ToList();

            var pagosTotales = _contexto.Pagos
                .Where(p => creditosAnteriores.Select(cr => cr.idObligacion).Contains(p.idObligacion))
                .ToList();

            // OPCIÓN 1: USAR MODELO ML (si existe y hay datos)
            if (File.Exists("modelo_crediticio.pkl") && creditosAnteriores.Count > 0)
            {
                try
                {
                    var prediccion = PredecirConModelo(ruc);
                    if (prediccion != null)
                    {
                        return MapearPrediccion(prediccion, config);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al usar modelo ML: {ex.Message}");
                    // Continuar con reglas de negocio
                }
            }

            // OPCIÓN 2: REGLAS DE NEGOCIO (fallback)
            return CalcularConReglas(ruc, config, creditosAnteriores, pagosTotales);
        }

        /// <summary>
        /// Predice usando el modelo ML de Python
        /// </summary>
        private PrediccionML PredecirConModelo(string ruc)
        {
            try
            {
                var proceso = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = _pythonPath,
                        Arguments = $"\"{_scriptPath}\" \"{ruc}\"",
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };

                proceso.Start();
                string output = proceso.StandardOutput.ReadToEnd();
                string error = proceso.StandardError.ReadToEnd();
                proceso.WaitForExit();

                if (proceso.ExitCode != 0)
                {
                    Console.WriteLine($"Error Python: {error}");
                    return null;
                }

                return JsonSerializer.Deserialize<PrediccionML>(output);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error ejecutando Python: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Calcula riesgo usando reglas de negocio
        /// </summary>
        private RecomendacionIA CalcularConReglas(
            string ruc,
            ConfiguracionIA config,
            List<Credito> creditos,
            List<Pago> pagos)
        {
            if (creditos.Count == 0)
            {
                return new RecomendacionIA
                {
                    NivelRiesgo = "ALTO",
                    InteresRecomendado = config.interesRiesgoAlto,
                    CuotasRecomendadas = config.cuotasMinimasPorDefecto,
                    Razonamiento = "Cliente sin historial crediticio. Se recomienda precaución con crédito inicial pequeño.",
                    UsaModelo = false
                };
            }

            var totalCuotasEsperadas = creditos.Sum(c => c.cuotas);
            var cuotasPagadas = pagos.Count;
            var tasaCumplimiento = totalCuotasEsperadas > 0
                ? (decimal)cuotasPagadas / totalCuotasEsperadas
                : 0;

            var pagosTardios = pagos.Count(p =>
            {
                var credito = creditos.FirstOrDefault(c => c.idObligacion == p.idObligacion);
                if (credito == null) return false;
                return (p.fechaPago - credito.fechaRegistro).Days > config.diasParaMorosidad;
            });

            var tasaPuntualidad = pagos.Count > 0
                ? 1 - ((decimal)pagosTardios / pagos.Count)
                : 0;

            var score = (tasaCumplimiento * 50) + (tasaPuntualidad * 50);

            if (score >= 80)
            {
                return new RecomendacionIA
                {
                    NivelRiesgo = "BAJO",
                    InteresRecomendado = config.interesRiesgoBajo,
                    CuotasRecomendadas = config.cuotasMaximasPorDefecto,
                    Razonamiento = $"Excelente historial de pagos ({score:F0}% de confiabilidad). Cliente preferencial con {creditos.Count} créditos pagados a tiempo.",
                    UsaModelo = false
                };
            }
            else if (score >= 50)
            {
                return new RecomendacionIA
                {
                    NivelRiesgo = "MEDIO",
                    InteresRecomendado = config.interesRiesgoMedio,
                    CuotasRecomendadas = (config.cuotasMinimasPorDefecto + config.cuotasMaximasPorDefecto) / 2,
                    Razonamiento = $"Historial aceptable ({score:F0}% de confiabilidad). Algunos retrasos en pagos anteriores. Se recomienda monitoreo.",
                    UsaModelo = false
                };
            }
            else
            {
                return new RecomendacionIA
                {
                    NivelRiesgo = "ALTO",
                    InteresRecomendado = config.interesRiesgoAlto,
                    CuotasRecomendadas = config.cuotasMinimasPorDefecto,
                    Razonamiento = $"Historial deficiente ({score:F0}% de confiabilidad). Múltiples retrasos o incumplimientos. Alto riesgo de impago.",
                    UsaModelo = false
                };
            }
        }

        private RecomendacionIA MapearPrediccion(PrediccionML prediccion, ConfiguracionIA config)
        {
            string nivelRiesgo = prediccion.Riesgo switch
            {
                0 => "BAJO",
                1 => "MEDIO",
                _ => "ALTO"
            };

            decimal interes = prediccion.Riesgo switch
            {
                0 => config.interesRiesgoBajo,
                1 => config.interesRiesgoMedio,
                _ => config.interesRiesgoAlto
            };

            int cuotas = prediccion.Riesgo switch
            {
                0 => config.cuotasMaximasPorDefecto,
                1 => (config.cuotasMinimasPorDefecto + config.cuotasMaximasPorDefecto) / 2,
                _ => config.cuotasMinimasPorDefecto
            };

            return new RecomendacionIA
            {
                NivelRiesgo = nivelRiesgo,
                InteresRecomendado = interes,
                CuotasRecomendadas = cuotas,
                Razonamiento = $"Predicción basada en modelo ML (confianza: {prediccion.Probabilidad:P0}). {prediccion.Razonamiento}",
                UsaModelo = true
            };
        }
    }

    // Modelos auxiliares
    public class RecomendacionIA
    {
        public string NivelRiesgo { get; set; }
        public decimal InteresRecomendado { get; set; }
        public int CuotasRecomendadas { get; set; }
        public string Razonamiento { get; set; }
        public bool UsaModelo { get; set; }
    }

    public class PrediccionML
    {
        public int Riesgo { get; set; }
        public decimal Probabilidad { get; set; }
        public string Razonamiento { get; set; }
    }
}