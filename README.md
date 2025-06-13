Запуск проекта:

1. Ввод команды в терминал:

   `docker compose up --build`

2. Адрес для использования swagger:

  `http://localhost:5000/swagger`


Для удобного взаимодействия с БД можно использовать PgAdmin:

1. Перейти по адресу:

  `http://localhost:8080`

2. Зайти под логином/паролем: admin@admin.com / admin (переменные можно менять в docker-compose:

  `pgadmin:
    image: dpage/pgadmin4
    environment:
      PGADMIN_DEFAULT_EMAIL: admin@admin.com
      PGADMIN_DEFAULT_PASSWORD: admin
    ports:
      - "8080:80"
    depends_on:
      - postgres
    networks:
      - chatnet`

3. Добавить сервер:

Host: postgres (это имя сервиса в сети Docker)
User: postgres
Password: 1234
DB: chatapp_test

Переменные взяты из:

  `chatapp:
    image: chatapp
    build:
      context: .
      dockerfile: ChatApp.API/Dockerfile
    depends_on:
      - redis
      - postgres
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - ASPNETCORE_URLS=http://+:80
      - ConnectionStrings__Redis=redis:6379
      - ConnectionStrings__ChatAppDbContext=Host=postgres;User ID=postgres;Password=1234;Port=5432;Database=chatapp_test;
    ports:
      - "5000:80"
    networks:
      - chatnet`

    
