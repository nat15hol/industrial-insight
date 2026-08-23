import json
import os
import requests

REPO = "nat15hol/industrial-insight"
TOKEN = os.environ["GITHUB_TOKEN"]
HEADERS = {"Authorization": f"Bearer {TOKEN}", "Accept": "application/vnd.github+json"}

def fetch_all_issues():
    all_issues = []
    page = 1
    while True:
        resp = requests.get(
            f"https://api.github.com/repos/{REPO}/issues",
            headers=HEADERS,
            params={"state": "all", "per_page": 100, "page": page},
        )
        resp.raise_for_status()
        batch = resp.json()
        all_issues.extend(i for i in batch if "pull_request" not in i)
        if len(batch) < 100:
            break
        page += 1
    return all_issues

if __name__ == "__main__":
    issues = fetch_all_issues()
    os.makedirs("dashboard", exist_ok=True)
    with open("dashboard/data.json", "w") as f:
        json.dump(issues, f, indent=2)