#!/usr/bin/env python3
"""
Script to detect duplicate type definitions in C# codebase.
Finds types defined multiple times in the same namespace.
"""

import os
import re
from pathlib import Path
from collections import defaultdict
from typing import Dict, List, Tuple

# Pattern to match type definitions
TYPE_PATTERNS = [
    r'public\s+(?:readonly\s+)?(?:record\s+struct|record\s+class|record|class|struct|interface|enum)\s+(\w+)',
    r'public\s+(?:readonly\s+)?(?:record\s+struct|record\s+class|record|class|struct|interface|enum)\s+(\w+)\s*<',
    r'public\s+(?:readonly\s+)?(?:record\s+struct|record\s+class|record|class|struct|interface|enum)\s+(\w+)\s*\(',
]

# Pattern to match namespace
NAMESPACE_PATTERN = r'namespace\s+([\w\.]+);?'


def extract_namespace(content: str) -> str:
    """Extract namespace from file content."""
    match = re.search(NAMESPACE_PATTERN, content)
    return match.group(1) if match else "Unknown"


def extract_types(content: str, filepath: str) -> List[Tuple[str, int, str]]:
    """
    Extract type definitions from file content.
    Returns list of (type_name, line_number, filepath) tuples.
    """
    types = []
    lines = content.split('\n')
    
    for i, line in enumerate(lines, start=1):
        # Try each pattern
        for pattern in TYPE_PATTERNS:
            match = re.search(pattern, line)
            if match:
                type_name = match.group(1)
                types.append((type_name, i, filepath))
                break
    
    return types


def scan_directory(directory: Path) -> Dict[Tuple[str, str], List[Tuple[str, int]]]:
    """
    Scan directory for C# files and extract type definitions.
    Returns dict: {(namespace, type_name): [(filepath, line_number), ...]}
    """
    type_locations = defaultdict(list)
    
    for cs_file in directory.rglob('*.cs'):
        try:
            content = cs_file.read_text(encoding='utf-8')
            namespace = extract_namespace(content)
            types = extract_types(content, str(cs_file.relative_to(directory)))
            
            for type_name, line_num, filepath in types:
                key = (namespace, type_name)
                type_locations[key].append((filepath, line_num))
        except Exception as e:
            print(f"Error processing {cs_file}: {e}", file=os.sys.stderr)
    
    return type_locations


def find_duplicates(type_locations: Dict[Tuple[str, str], List[Tuple[str, int]]]) -> Dict[str, List[Tuple[str, int]]]:
    """
    Find types that are defined multiple times.
    Returns dict: {type_name: [(filepath, line_number), ...]}
    """
    duplicates = {}
    
    for (namespace, type_name), locations in type_locations.items():
        if len(locations) > 1:
            # Group by namespace to show which namespace has duplicates
            key = f"{namespace}.{type_name}"
            duplicates[key] = locations
    
    return duplicates


def main():
    # Default to Domain project
    domain_path = Path(__file__).parent.parent / "src" / "code" / "SemanticRag" / "IndFusion.SemanticRag.Domain"
    
    if not domain_path.exists():
        print(f"Domain project not found at: {domain_path}")
        print("Usage: python find_duplicate_types.py [path_to_domain_project]")
        return
    
    print(f"Scanning: {domain_path}")
    print("=" * 80)
    
    type_locations = scan_directory(domain_path)
    duplicates = find_duplicates(type_locations)
    
    if not duplicates:
        print("✓ No duplicate type definitions found!")
        return
    
    print(f"\nFound {len(duplicates)} duplicate type definitions:\n")
    
    # Sort by type name for easier reading
    sorted_duplicates = sorted(duplicates.items(), key=lambda x: (len(x[1]), x[0]))
    
    for type_key, locations in sorted_duplicates:
        print(f"\n{type_key} ({len(locations)} definitions):")
        for filepath, line_num in sorted(locations):
            print(f"  - {filepath}:{line_num}")
    
    # Summary by file
    print("\n" + "=" * 80)
    print("Summary by file (files with most duplicates):")
    print("=" * 80)
    
    file_counts = defaultdict(int)
    for locations in duplicates.values():
        for filepath, _ in locations:
            file_counts[filepath] += 1
    
    for filepath, count in sorted(file_counts.items(), key=lambda x: x[1], reverse=True)[:20]:
        print(f"  {filepath}: {count} duplicate(s)")


if __name__ == "__main__":
    import sys
    if len(sys.argv) > 1:
        domain_path = Path(sys.argv[1])
        if not domain_path.exists():
            print(f"Path not found: {domain_path}")
            sys.exit(1)
    main()

