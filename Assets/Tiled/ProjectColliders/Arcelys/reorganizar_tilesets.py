#!/usr/bin/env python3
"""
Script para reorganizar tilesets de Tiled para Unity/Tiled2Unity
Copia TODAS las imágenes referenciadas en archivos .tsx a una carpeta local 'textures/'
y actualiza las rutas en los archivos .tsx para usar rutas relativas simples.
"""

import os
import re
import shutil
import xml.etree.ElementTree as ET
from pathlib import Path
import sys

def resolve_image_path(source_path, tsx_file_path):
    """
    Resuelve la ruta absoluta de una imagen desde un archivo .tsx
    
    Args:
        source_path: Ruta de la imagen como aparece en el .tsx
        tsx_file_path: Ruta absoluta del archivo .tsx
    
    Returns:
        Ruta absoluta resuelta de la imagen
    """
    tsx_dir = os.path.dirname(tsx_file_path)
    
    # Si la ruta comienza con ~, expandir el home del usuario
    if source_path.startswith('~'):
        source_path = os.path.expanduser(source_path)
    
    # Si es ruta absoluta, usarla directamente
    if os.path.isabs(source_path):
        return source_path
    
    # Si no, es ruta relativa al directorio del .tsx
    # Intentar diferentes combinaciones de rutas relativas
    possible_paths = [
        os.path.join(tsx_dir, source_path),
        os.path.normpath(os.path.join(tsx_dir, source_path))
    ]
    
    for path in possible_paths:
        if os.path.exists(path):
            return os.path.abspath(path)
    
    # Si no se encuentra, devolver la ruta construida (para reportar error)
    return os.path.abspath(os.path.join(tsx_dir, source_path))

def copy_image_to_textures(image_path, textures_dir, used_names):
    """
    Copia una imagen a la carpeta textures/, manejando nombres duplicados
    
    Args:
        image_path: Ruta absoluta de la imagen original
        textures_dir: Directorio de destino para textures/
        used_names: Diccionario de nombres ya usados para manejar duplicados
    
    Returns:
        Tupla (nombre_final, éxito, mensaje)
    """
    try:
        if not os.path.exists(image_path):
            return None, False, f"Archivo no encontrado: {image_path}"
        
        # Obtener nombre del archivo
        original_name = os.path.basename(image_path)
        name, ext = os.path.splitext(original_name)
        
        # Determinar nombre final
        if original_name in used_names:
            # Si el nombre ya existe, agregar sufijo numérico
            counter = used_names[original_name]
            used_names[original_name] += 1
            final_name = f"{name}_{counter}{ext}"
        else:
            final_name = original_name
            used_names[original_name] = 1
        
        # Ruta destino
        dest_path = os.path.join(textures_dir, final_name)
        
        # Copiar archivo
        shutil.copy2(image_path, dest_path)
        
        return final_name, True, f"Copiado como {final_name}"
    
    except Exception as e:
        return None, False, f"Error al copiar {image_path}: {str(e)}"

def process_tsx_file(tsx_path, textures_dir, used_names, stats):
    """
    Procesa un archivo .tsx individual
    
    Args:
        tsx_path: Ruta del archivo .tsx
        textures_dir: Directorio de textures/
        used_names: Diccionario de nombres usados
        stats: Diccionario para estadísticas
    
    Returns:
        Lista de cambios realizados
    """
    changes = []
    
    try:
        # Parsear el archivo XML
        tree = ET.parse(tsx_path)
        root = tree.getroot()
        
        # Buscar todas las etiquetas <image>
        for image_elem in root.findall('.//image'):
            source_attr = image_elem.get('source')
            if not source_attr:
                continue
            
            # Saltar si ya está en textures/ o es ruta local simple
            if source_attr.startswith('textures/') or (not any(x in source_attr for x in ['../', '~', '/'])) and not source_attr.startswith('http'):
                stats['skipped'] += 1
                continue
            
            # Resolver ruta original
            original_path = resolve_image_path(source_attr, tsx_path)
            
            # Copiar imagen a textures/
            new_name, success, message = copy_image_to_textures(original_path, textures_dir, used_names)
            
            if success:
                # Actualizar ruta en el XML
                new_source = f"textures/{new_name}"
                old_source = source_attr
                image_elem.set('source', new_source)
                
                changes.append({
                    'old': old_source,
                    'new': new_source,
                    'image': new_name,
                    'message': message
                })
                
                stats['copied'] += 1
            else:
                changes.append({
                    'old': source_attr,
                    'new': None,
                    'image': None,
                    'message': f"ERROR: {message}"
                })
                
                stats['errors'] += 1
        
        # Guardar cambios si hubo modificaciones
        if changes:
            tree.write(tsx_path, encoding='UTF-8', xml_declaration=True)
            stats['updated_files'] += 1
        
        return changes
    
    except ET.ParseError as e:
        error_msg = f"Error parsing XML: {str(e)}"
        print(f"  ERROR en {tsx_path}: {error_msg}")
        changes.append({
            'old': '',
            'new': None,
            'image': None,
            'message': f"ERROR: {error_msg}"
        })
        stats['errors'] += 1
        return changes
    
    except Exception as e:
        error_msg = f"Error procesando archivo: {str(e)}"
        print(f"  ERROR en {tsx_path}: {error_msg}")
        changes.append({
            'old': '',
            'new': None,
            'image': None,
            'message': f"ERROR: {error_msg}"
        })
        stats['errors'] += 1
        return changes

def find_tsx_files(start_dir):
    """
    Encuentra todos los archivos .tsx en un directorio y subdirectorios
    
    Args:
        start_dir: Directorio de inicio para la búsqueda
    
    Returns:
        Lista de rutas absolutas de archivos .tsx
    """
    tsx_files = []
    
    for root, dirs, files in os.walk(start_dir):
        # Excluir la carpeta textures/ si existe
        if 'textures' in dirs:
            dirs.remove('textures')
        
        for file in files:
            if file.lower().endswith('.tsx'):
                tsx_files.append(os.path.join(root, file))
    
    return tsx_files

def generate_report(stats, all_changes, textures_dir, log_file):
    """
    Genera un reporte detallado del proceso
    
    Args:
        stats: Estadísticas del proceso
        all_changes: Lista de todos los cambios por archivo
        textures_dir: Ruta de la carpeta textures/
        log_file: Archivo para escribir el log
    """
    with open(log_file, 'w', encoding='utf-8') as f:
        f.write("=" * 70 + "\n")
        f.write("REPORTE DE REORGANIZACIÓN DE TILESETS\n")
        f.write("=" * 70 + "\n\n")
        
        f.write(f"Fecha y hora: {stats['timestamp']}\n")
        f.write(f"Directorio base: {stats['base_dir']}\n")
        f.write(f"Carpeta textures: {textures_dir}\n\n")
        
        f.write("-" * 70 + "\n")
        f.write("ESTADÍSTICAS\n")
        f.write("-" * 70 + "\n")
        f.write(f"Archivos .tsx procesados: {stats['processed_files']}\n")
        f.write(f"Archivos .tsx actualizados: {stats['updated_files']}\n")
        f.write(f"Imágenes copiadas: {stats['copied']}\n")
        f.write(f"Imágenes omitidas (ya en textures/): {stats['skipped']}\n")
        f.write(f"Errores encontrados: {stats['errors']}\n\n")
        
        f.write("-" * 70 + "\n")
        f.write("DETALLE POR ARCHIVO\n")
        f.write("-" * 70 + "\n")
        
        for file_changes in all_changes:
            if not file_changes['changes']:
                continue
            
            f.write(f"\n{file_changes['file']}:\n")
            f.write(f"{'-' * 50}\n")
            
            for change in file_changes['changes']:
                if change['new']:
                    f.write(f"  ✓ {change['old']}\n")
                    f.write(f"    → {change['new']} ({change['message']})\n")
                else:
                    f.write(f"  ✗ ERROR: {change['message']}\n")
            f.write("\n")
        
        f.write("-" * 70 + "\n")
        f.write("ARCHIVOS COPIADOS A TEXTURES/\n")
        f.write("-" * 70 + "\n")
        
        if os.path.exists(textures_dir):
            images = sorted(os.listdir(textures_dir))
            if images:
                for img in images:
                    f.write(f"  • {img}\n")
                f.write(f"\nTotal: {len(images)} archivos\n")
            else:
                f.write("  (No hay archivos)\n")
        else:
            f.write("  (La carpeta textures/ no existe)\n")
        
        f.write("\n" + "=" * 70 + "\n")
        f.write("PROCESO COMPLETADO\n")
        
        if stats['errors'] == 0:
            f.write("✓ ¡Todas las imágenes fueron procesadas exitosamente!\n")
        else:
            f.write(f"⚠ Se encontraron {stats['errors']} errores. Revisa el log.\n")
        
        f.write("=" * 70 + "\n")

def main():
    """Función principal del script"""
    
    print("=" * 70)
    print("REORGANIZADOR DE TILESETS PARA TILED2UNITY")
    print("=" * 70)
    
    # Obtener directorio actual
    base_dir = os.getcwd()
    print(f"Directorio actual: {base_dir}")
    
    # Crear carpeta textures/ si no existe
    textures_dir = os.path.join(base_dir, "textures")
    if not os.path.exists(textures_dir):
        os.makedirs(textures_dir)
        print(f"Creada carpeta: {textures_dir}")
    else:
        print(f"Carpeta textures/ ya existe: {textures_dir}")
    
    # Encontrar todos los archivos .tsx
    print("\nBuscando archivos .tsx...")
    tsx_files = find_tsx_files(base_dir)
    
    if not tsx_files:
        print("No se encontraron archivos .tsx en el directorio actual.")
        return
    
    print(f"Encontrados {len(tsx_files)} archivo(s) .tsx")
    
    # Inicializar estadísticas
    stats = {
        'timestamp': "2024",
        'base_dir': base_dir,
        'processed_files': 0,
        'updated_files': 0,
        'copied': 0,
        'skipped': 0,
        'errors': 0
    }
    
    # Diccionario para manejar nombres duplicados
    used_names = {}
    
    # Procesar cada archivo .tsx
    all_changes = []
    
    print("\nProcesando archivos...")
    print("-" * 70)
    
    for tsx_file in tsx_files:
        print(f"\nProcesando: {os.path.basename(tsx_file)}")
        
        changes = process_tsx_file(tsx_file, textures_dir, used_names, stats)
        stats['processed_files'] += 1
        
        all_changes.append({
            'file': tsx_file,
            'changes': changes
        })
        
        # Mostrar resumen de cambios en este archivo
        if changes:
            success_count = sum(1 for c in changes if c['new'])
            error_count = sum(1 for c in changes if not c['new'])
            
            if success_count > 0:
                print(f"  ✓ {success_count} imagen(es) actualizada(s)")
            if error_count > 0:
                print(f"  ✗ {error_count} error(es)")
        else:
            print("  (Sin cambios necesarios)")
    
    # Generar reporte
    print("\n" + "=" * 70)
    print("GENERANDO REPORTE...")
    
    log_file = os.path.join(base_dir, "tileset_reorganization_log.txt")
    generate_report(stats, all_changes, textures_dir, log_file)
    
    print(f"Reporte guardado en: {log_file}")
    print("\n" + "=" * 70)
    print("RESUMEN FINAL:")
    print(f"• Archivos .tsx procesados: {stats['processed_files']}")
    print(f"• Archivos actualizados: {stats['updated_files']}")
    print(f"• Imágenes copiadas a textures/: {stats['copied']}")
    print(f"• Imágenes en textures/: {len(os.listdir(textures_dir)) if os.path.exists(textures_dir) else 0}")
    print(f"• Errores: {stats['errors']}")
    
    if stats['errors'] == 0:
        print("\n✅ ¡PROCESO COMPLETADO EXITOSAMENTE!")
        print("\nTu proyecto ahora tiene:")
        print("1. Carpeta 'textures/' con TODAS las imágenes copiadas")
        print("2. Archivos .tsx actualizados con rutas 'textures/nombre.png'")
        print("3. Archivos .tmx listos (no requieren cambios)")
        print("\nPuedes copiar toda la carpeta a Unity y usar Tiled2Unity sin errores de rutas.")
    else:
        print(f"\n⚠ Proceso completado con {stats['errors']} error(es).")
        print("Revisa el archivo de log para más detalles.")
    
    print("=" * 70)

if __name__ == "__main__":
    # Verificar que estamos en el directorio correcto
    print("ADVERTENCIA: Este script modificará archivos .tsx en el directorio actual.")
    print("Se recomienda hacer una copia de seguridad antes de continuar.")
    
    respuesta = input("\n¿Continuar? (s/n): ").strip().lower()
    
    if respuesta == 's':
        main()
    else:
        print("Operación cancelada.")
        sys.exit(0)
