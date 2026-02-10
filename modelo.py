#pip install mysql-connector-python scikit-learn pandas numpy joblib sqlalchemy pymysql

import mysql.connector
import pandas as pd
import numpy as np
from sklearn.model_selection import train_test_split
from sklearn.ensemble import RandomForestClassifier
from sklearn.preprocessing import StandardScaler
from sqlalchemy import create_engine
import joblib
import warnings
warnings.filterwarnings('ignore')

print("=" * 50)
print("=" * 50)

engine = create_engine('mysql+mysqlconnector://root:@localhost/ProyectoIntegradorDB')

query = """
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
GROUP BY cl.ruc
HAVING total_creditos > 0;
"""

print("\n📥 Cargando datos desde MySQL...")
df = pd.read_sql(query, engine)
print(f"✅ Datos cargados: {len(df)} clientes con historial")

if len(df) == 0:
    print("\n⚠️  No hay suficientes datos para entrenar el modelo.")
    print("💡 Se usarán reglas de negocio por defecto.")
    print("\nCrea al menos 5 créditos con pagos para entrenar el modelo.")
    exit()

# FEATURE ENGINEERING
print("\n🔧 Generando características...")
df['tasa_cumplimiento'] = df['total_pagos_realizados'] / (df['total_creditos'] * 7).replace(0, 1)
df['tasa_puntualidad'] = 1 - (df['pagos_tardios'] / df['total_pagos_realizados'].replace(0, 1))
df['monto_promedio_credito'] = df['monto_total_historico'] / df['total_creditos'].replace(0, 1)

# CALCULAR RIESGO (etiqueta)
def calcular_riesgo(row):
    if row['total_creditos'] == 0:
        return 2  # ALTO
    
    # Score basado en comportamiento
    score = (row['tasa_cumplimiento'] * 50) + (row['tasa_puntualidad'] * 50)
    
    if score >= 80:
        return 0  # BAJO
    elif score >= 50:
        return 1  # MEDIO
    else:
        return 2  # ALTO

df['riesgo'] = df.apply(calcular_riesgo, axis=1)

print(f"  - Riesgo BAJO: {(df['riesgo'] == 0).sum()} clientes")
print(f"  - Riesgo MEDIO: {(df['riesgo'] == 1).sum()} clientes")
print(f"  - Riesgo ALTO: {(df['riesgo'] == 2).sum()} clientes")

# PREPARAR DATOS
features = [
    'total_creditos',
    'monto_promedio_credito',
    'tasa_cumplimiento',
    'tasa_puntualidad',
    'dias_desde_primer_credito',
    'saldoPendiente'
]

X = df[features].fillna(0)
y = df['riesgo']

# VERIFICAR SI HAY SUFICIENTES DATOS
if len(df) < 5:
    print(f"\n⚠️  Solo hay {len(df)} registros. Mínimo recomendado: 5")
    print("💡 El modelo se creará pero su precisión será baja.")
    print("   Agrega más datos para mejorar las predicciones.\n")

# DIVIDIR DATOS (si hay suficientes)
if len(df) >= 5:
    X_train, X_test, y_train, y_test = train_test_split(
        X, y, test_size=0.2, random_state=42, stratify=y if len(df) >= 10 else None
    )
else:
    # Usar todos los datos para entrenar
    X_train, X_test = X, X
    y_train, y_test = y, y
    print("⚠️  Usando todos los datos para entrenamiento (sin validación)\n")

# NORMALIZAR
print("📊 Normalizando características...")
scaler = StandardScaler()
X_train_scaled = scaler.fit_transform(X_train)
X_test_scaled = scaler.transform(X_test)

# ENTRENAR MODELO
print("🧠 Entrenando modelo Random Forest...")
model = RandomForestClassifier(
    n_estimators=50,  # Reducido para datasets pequeños
    max_depth=5,      # Evitar overfitting
    random_state=42,
    min_samples_split=2,
    min_samples_leaf=1
)
model.fit(X_train_scaled, y_train)

# EVALUAR
score = model.score(X_test_scaled, y_test)
print(f"\n✅ Modelo entrenado!")
print(f"📈 Precisión: {score * 100:.2f}%")

# IMPORTANCIA DE CARACTERÍSTICAS
importancias = pd.DataFrame({
    'caracteristica': features,
    'importancia': model.feature_importances_
}).sort_values('importancia', ascending=False)

print("\n📊 Importancia de características:")
for idx, row in importancias.iterrows():
    print(f"  - {row['caracteristica']}: {row['importancia']:.2%}")

# GUARDAR MODELO
print("\n💾 Guardando modelo y scaler...")
joblib.dump(model, 'modelo_crediticio.pkl')
joblib.dump(scaler, 'scaler.pkl')

# GUARDAR METADATA
metadata = {
    'fecha_entrenamiento': pd.Timestamp.now().isoformat(),
    'num_registros': len(df),
    'accuracy': float(score),
    'features': features,
    'distribucion_riesgo': {
        'BAJO': int((df['riesgo'] == 0).sum()),
        'MEDIO': int((df['riesgo'] == 1).sum()),
        'ALTO': int((df['riesgo'] == 2).sum())
    }
}

import json
with open('modelo_metadata.json', 'w') as f:
    json.dump(metadata, f, indent=2)

print("✅ Archivos guardados:")
print("  - modelo_crediticio.pkl")
print("  - scaler.pkl")
print("  - modelo_metadata.json")

print("\n" + "=" * 50)
print("✅ PROCESO COMPLETADO")
print("=" * 50)