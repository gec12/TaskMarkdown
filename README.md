# TaskMarkdown
Тестовое задание ЛЭТИ
Данное консольное приложение:
- принимает список файлов формата Markdown в качестве аргументов командной строки;
- проводит подсчет суммарного количества изображений и таблиц в файлах;
- проверяет для каждого файла, есть ли у каждого изображения или таблицы подпись вида "Рисунок 1. ....", "Таблица 2. ....", выводит список неподписанных;
- результат упакован в Docker-контейнер.

На вход в качестве аргумента командной строки принимается путь к папке с файлами.
Принято что подпись к таблицам/изображениям находится на строке перед самими объектом, с выравниваем слева.
