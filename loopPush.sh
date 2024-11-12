#!/bin/bash

# 최대 용량 설정 (1.9GB)
MAX_SIZE=$((1900 * 1024 * 1024))  # 1.9GB in bytes
REPO_PATH="/path/to/your/repo"    # 저장소 경로를 설정하세요.
COMMIT_MESSAGE="Batch commit"     # 커밋 메시지
BRANCH_NAME="main"                # 브랜치 이름을 설정하세요 (예: main, master 등)

# Git 저장소로 이동
#cd "$REPO_PATH" || exit

# 진행 상황과 로그 출력을 위한 함수
print_status() {
    echo "Current status:"
    git status
    echo -e "\nRecent commits:"
    git log --oneline -5
    echo "-----------------------------------"
}

# 커밋과 푸시 작업 반복
while true; do
    # 스테이징 크기 초기화
    current_size=0

    # 파일을 하나씩 스테이징하여 크기 계산
    for file in $(git ls-files --others --exclude-standard); do
        file_size=$(stat -c%s "$file")

        # 현재 파일을 추가했을 때 1.9GB 초과하는 경우 반복문 종료
        if (( current_size + file_size > MAX_SIZE )); then
            break
        fi

        # 파일을 스테이징하고 크기 합산
        git add "$file"
        current_size=$((current_size + file_size))
    done

    # 스테이징할 파일이 없으면 종료
    if (( current_size == 0 )); then
        echo "All files are committed and pushed."
        break
    fi

    # 커밋 및 푸시
    echo "Committing and pushing batch ($current_size bytes)"
    git commit -m "$COMMIT_MESSAGE"
    git push origin main
    echo "Batch committed and pushed."

    # 진행 상황 및 로그 출력
    print_status
done

