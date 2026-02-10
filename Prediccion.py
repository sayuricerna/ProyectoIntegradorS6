import sys
import joblib
import mysql.connector
import pandas as pd
import numpy as np
import json
from sqlalchemy import create_engine

def predecir_riesgo(ruc):
    try:
        # Cargar modelo y scaler
        model = joblib.load('modelo_crediticio.pkl')
        scaler = joblib.load('scaler.pkl')
        
        # Conectar a BD
        engine = create_engine('mysql+mysqlconnector://root:@localhost/ProyectoIntegradorDB')
        
        # Obtener datos del cliente
        query = f"""
        SELECT 
            cl.ruc,
            COUNT(DISTINCT cr.idObligacion) as total_creditos,
            COALESCE(SUM(cr.montoTotal), 0) as monto_total_historico,
            COALESCE(AVG(cr.tasaInteres), 0) as interes_promedio,
            COUNT(p.id) as total_pagos_realizados,
            SUM(CASE 
                WHEN DATEDIFF(p.fechaPago, cr.fechaRegistro) > 30 
                THEN 1 ELSE 0 
            END) as pagos_tardios,
            COALESCE(MAX(DATEDIFF(NOW(), cr.fechaRegistro)), 0) as dias_desde_primer_credito,
            cl.saldoPendiente
        FROM clientes cl
        LEFT JOIN creditos cr ON cl.ruc = cr.rucCliente
        LEFT JOIN pagos p ON cr.idObligacion = p.idObligacion
        WHERE cl.ruc = '{ruc}'
        GROUP BY cl.ruc;
        """
        
        df = pd.read_sql(query, engine)
        
        if len(df) == 0:
            return {
                "Riesgo": 2,
                "Probabilidad": 1.0,
                "Razonamiento": "Cliente sin datos"
            }
        
        # Calcular features
        df['tasa_cumplimiento'] = df['total_pagos_realizados'] / (df['total_creditos'] * 7 + 1)
        df['tasa_puntualidad'] = 1 - (df['pagos_tardios'] / df['total_pagos_realizados'].replace(0, 1))
        df['monto_promedio_credito'] = df['monto_total_historico'] / df['total_creditos'].replace(0, 1)
        
        features = [
            'total_creditos',
            'monto_promedio_credito',
            'tasa_cumplimiento',
            'tasa_puntualidad',
            'dias_desde_primer_credito',
            'saldoPendiente'
        ]
        
        X = df[features].fillna(0)
        X_scaled = scaler.transform(X)
        
        # Predecir
        prediccion = model.predict(X_scaled)[0]
        probabilidades = model.predict_proba(X_scaled)[0]
        
        resultado = {
            "Riesgo": int(prediccion),
            "Probabilidad": float(probabilidades[prediccion]),
            "Razonamiento": f"Basado en {int(df['total_creditos'].iloc[0])} créditos anteriores"
        }
        
        print(json.dumps(resultado))
        return resultado
        
    except Exception as e:
        print(json.dumps({
            "Riesgo": 2,
            "Probabilidad": 0.0,
            "Razonamiento": f"Error: {str(e)}"
        }), file=sys.stderr)
        sys.exit(1)

if __name__ == "__main__":
    if len(sys.argv) < 2:
        print("Uso: python prediccion.py <RUC>", file=sys.stderr)
        sys.exit(1)
    
    ruc = sys.argv[1]
    predecir_riesgo(ruc)