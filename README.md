# iOS Unity Build & TestFlight Upload Workflow

Этот репозиторий содержит GitHub Actions workflow для сборки Unity-проекта и загрузки iOS-билда в TestFlight.

## Необходимые переменные среды

Для успешного запуска workflow необходимо определить следующие переменные среды (Secrets/Variables) в настройках репозитория:

### Secrets

- `UNITY_LICENSE` — Лицензия Unity.
- `UNITY_EMAIL` — Email пользователя Unity.
- `UNITY_PASSWORD` — Пароль пользователя Unity.
- `APPSTORE_KEY_ID` — App Store Connect API Key ID.
- `APPSTORE_ISSUER_ID` — App Store Connect Issuer ID.
- `APPSTORE_P8` — Содержимое приватного ключа App Store Connect (.p8).
- `MATCH_PASSWORD` — Пароль для match (Fastlane), может быть любым.
- `GH_PAT` — GitHub Personal Access Token (для доступа к приватному репозиторию сертификатов).

### Variables

- `UNITY_VERSION` — Версия Unity (например, 2022.3.62f3).
- `XCODE_VERSION` — Версия Xcode (например, 26.2).
- `BUNDLE_IDENTIFIER` — Bundle Identifier приложения.
- `APPLE_TEAM_ID` — Apple Team ID.
- `MATCH_GIT_URL` — URL репозитория для хранения сертификатов (дожен быть пустым).
- `APPLE_ID_DIGITS` — Apple ID (только цифры).
- `LAST_UPLOADED_BUILD_NUMBER` — Последний загруженный номер билда.
- `MAJOR_VERSION` — Основная версия приложения.

## Как использовать

1. Определите все необходимые secrets и variables в настройках репозитория.
2. Запустите workflow вручную через вкладку Actions, выбрав нужный тип сборки (`full` или `short`).

## Описание workflow

Workflow состоит из двух основных job:

- **buildForAllSupportedPlatforms** — сборка проекта Unity для iOS.
- **xcodeBuildAndUpload** — сборка Xcode и загрузка в TestFlight через Fastlane.

Подробнее см. в файле `.github/workflows/unity_ios.yml`.
