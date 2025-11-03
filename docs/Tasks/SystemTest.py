
def xml_escape(s: str) -> str:
    return s.replace("&", "&amp;").replace("<", "&lt;").replace(">", "&gt;")

import os
import re

PROJECT_ROOT = "F:\Dynamic\IndFusion\IndFusion.Mcp\ExxerRules\src"  # Replace with your actual source root
OUTPUT_FILE = "F:\Dynamic\IndFusion\IndFusion.Mcp\ExxerRules\docs\Tasks\XMLExxerRules_Checklist.md"

pattern_class = re.compile(r'^\s*(public|protected)\s+(?:abstract\s+|sealed\s+)?(class|interface|enum|struct)\s+(\w+)', re.MULTILINE)
pattern_method = re.compile(r'^\s*(public|protected)\s+(static\s+)?([\w<>\[\],\s]+)\s+(\w+)\s*\(([^)]*)\)', re.MULTILINE)
pattern_property = re.compile(r'^\s*(public|protected)\s+([\w<>\[\],\s]+)\s+(\w+)\s*{\s*(get|set)', re.MULTILINE)

checklist = []

def scan_file(filepath):
    with open(filepath, encoding="utf-8", errors="ignore") as f:
        content = f.read()
    rel_path = os.path.relpath(filepath, PROJECT_ROOT)

    # Classes
    for match in pattern_class.finditer(content):
        visibility, type_keyword, name = match.groups()
        checklist.append(f"- [ ] {type_keyword} `{name}` in `{rel_path}`")

    # Methods
    for match in pattern_method.finditer(content):
        visibility, static, return_type, name, args = match.groups()
        checklist.append(f"  - [ ] Method `{name}({args}) : {return_type}`")

    # Properties
    for match in pattern_property.finditer(content):
        visibility, prop_type, name, accessor = match.groups()
        checklist.append(f"  - [ ] Property `{name} : {prop_type}`")

def walk_project():
    for root, dirs, files in os.walk(PROJECT_ROOT):
        for file in files:
            if file.endswith(".cs"):
                scan_file(os.path.join(root, file))

if __name__ == "__main__":
    walk_project()
    with open(OUTPUT_FILE, "w", encoding="utf-8") as f:
        f.write("# XML Documentation Checklist\n\n")
        f.write("\n".join(checklist))
    print(f"Checklist written to {OUTPUT_FILE}")
