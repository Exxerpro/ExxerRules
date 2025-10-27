import os
import re
import subprocess
from pathlib import Path

def get_xunit1051_errors():
    solution_file = Path("IndFusion.SemanticRag.Tests.Unit/IndFusion.SemanticRag.Tests.Unit.csproj")
    
    try:
        result = subprocess.run(
            ["dotnet", "build", str(solution_file), "--no-restore", "-v:m"],
            capture_output=True,
            text=True,
            cwd="."
        )
        
        errors = []
        full_output = result.stderr + "\n" + result.stdout
        
        for line in full_output.split("\n"):
            if "xUnit1051" in line and ".cs(" in line:
                match = re.match(r"([^(]+)\((\d+),\d+\):\s*error\s+xUnit1051:", line)
                if match:
                    filepath = Path(match.group(1))
                    line_num = int(match.group(2))
                    errors.append({
                        "file": str(filepath),
                        "line": line_num,
                        "original_line": line.strip()
                    })
        
        print(f"Found {len(errors)} xUnit1051 errors")
        for error in errors[:10]:
            print(f"  {error[\"file\"]}:{error[\"line\"]} - {error[\"original_line\"]}")
        
        return errors
        
    except Exception as e:
        print(f"Error running build: {e}")
        return []

if __name__ == "__main__":
    get_xunit1051_errors()
