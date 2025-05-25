# Контрольная работа по КПО №2
## Синхронное межсерверное взаимодействие

###Набор из трёх микросервисов:

1. **FileStorageService** — хранение файлов и метаданных  
2. **FileAnalysisService** — подсчёт слов/символов, генерация word-cloud, вычисление схожести  
3. **APIGateway** — единая точка входа, маршрутизация запросов к storage и analysis

FileStorageService хранит в базе PostgreSQL таблицу Files и сохраняет бинарники на диск в /app/Storage.

FileAnalysisService хранит в той же БД таблицу Analyses, запрашивает контент у FileStorage и отдаёт JSON-анализ + PNG wordcloud.

APIGateway проксирует HTTP-запросы к двум сервисам (авторизация/кеширование можно добавить по желанию).

###Структура проекта:

├── APIGateway

│   ├── Gateway

├── FileStorageService

│   ├── Domain

│   ├── Infrastructure

│   ├── Presentation

├── FileAnalysisService

│   ├── Domain

│   ├── Application

│   ├── Infrastructure

│   ├── Presentation

└── docker-compose.yml



###Контейнеризация

В проекте используется Docker Compose для локального запуска
Сервисы в Docker:

•    postgres: база данных, порт 5432.

•    filestorage: FileStorageService на порту 5001.

•    fileanalysis: FileAnalysisService на порту 5002.

•    gateway: APIGateway на порту 8080.


###API Endpoints

Доступно через Gateway http://localhost:8080.

POST    /upload    Загрузить файл. Возвращает JSON StoredFile.

GET    /files/{id}    Метаданные файла.

GET    /files/{id}/content    Содержимое файла.

POST    /analyze/{fileId}    Запустить анализ файла (если не был проведён).

GET    /analyze/{fileId}    Получить результат анализа (JSON).

GET    /wordcloud/{analysisId}    Получить PNG word-cloud по результату.

GET    /compare/{a}/{b}    Сравнить два анализа, вернуть схожесть (JSON).


