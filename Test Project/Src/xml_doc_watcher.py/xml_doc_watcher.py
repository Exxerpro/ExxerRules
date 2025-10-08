import os
import time
import subprocess
from watchdog.observers import Observer
from watchdog.events import FileSystemEventHandler

class XmlCommentWatcher(FileSystemEventHandler):
    def __init__(self, repo_path):
        self.repo_path = repo_path
        self.modified_files = set()

    def on_modified(self, event):
        if event.is_directory or not event.src_path.endswith(".cs"):
            return
        try:
            with open(event.src_path, "r", encoding="utf-8") as file:
                content = file.read()
                if "/// <summary>" in content or "/// <" in content:
                    relative_path = os.path.relpath(event.src_path, self.repo_path)
                    self.modified_files.add(relative_path)
        except Exception:
            pass

    def commit_changes(self):
        if self.modified_files:
            subprocess.run(["git", "add", "."], cwd=self.repo_path)
            commit_msg = "Added XML comments: " + ", ".join(self.modified_files)
            subprocess.run(["git", "commit", "-m", commit_msg], cwd=self.repo_path)
            print(f"Committed: {commit_msg}")
            self.modified_files.clear()

def start_watching(path):
    handler = XmlCommentWatcher(path)
    observer = Observer()
    observer.schedule(handler, path, recursive=True)
    observer.start()
    print(f"Watching for XML comments in: {path}")
    try:
        while True:
            time.sleep(10)
            handler.commit_changes()
    except KeyboardInterrupt:
        observer.stop()
    observer.join()

if __name__ == "__main__":
    repo_dir = os.path.abspath(".")
    start_watching(repo_dir)
