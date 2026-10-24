#!/bin/zsh -l
# Сборка и запуск тестов. Результат пишется в test_output.txt
cd "$(dirname "$0")"
export PATH="$HOME/.dotnet:/usr/local/share/dotnet:/opt/homebrew/bin:/usr/local/bin:$PATH"
{
  echo "=== $(date)"
  echo "--- dotnet: $(command -v dotnet)"
  dotnet --list-sdks
  dotnet --list-runtimes
  echo "--- build"
  dotnet build
  echo "BUILD_EXIT $?"
  echo "--- test"
  dotnet test --collect:"XPlat Code Coverage" --results-directory ./TestResults
  echo "TEST_EXIT $?"
} 2>&1 | tee test_output.txt
echo
echo "Готово, окно можно закрыть."
