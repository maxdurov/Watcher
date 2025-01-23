# Watcher

Данная программа предназначена для мониторинга сетевой инфраструктуры. Используйте ее на свой страх и риск.

TERMS AND CONDITIONS FOR USE, REPRODUCTION, AND DISTRIBUTION:  Apache License 2.0

![Watcher](https://i.ibb.co/qxHsQvw/prevy.png)

# Быстрый старт

ПРИМЕЧАНИЕ. После первого запуска программы произойдет инициализация конфигурационного файла. Для избежания проблем храните программу в отдельной папке и не изменяйте конфигурационный файл. В случае его повреждения программа предложит создать новый. В таком случае все настройки будут сброшены.

Для начала работы необходимо сделать несколько действий:

     1. Зайти в настройки

     2. Добавить новый объект для мониторинга

     3. Сохранить и вернуться на главную страницу

## Описание страницы добавления объекта и возможностей

Название - поле для названия объекта. Должно быть уникальным. Например: SERVER-1, LOCAL SITE и т. д.

HOST/IP адрес - поле для ввода адреса, по которому будет происходить проверка доступности. Если возникает необходимость обратиться к HOST определенного IP-адреса, то можно ввести в это поле IP-адрес, а заголовок HOST добавить в заголовки запроса (см. Настройка Headers (GET, POST)).

Метод - список доступных технологий для обращения к объекту.

         1. GET (CODE) - отправляет GET HTTP/S запрос по заданному адресу и сравнивает только возвращаемый ответ (RESPONSE STATUS. См. RFC 9110) с заданным значением в поле Ответ (RAW).

         2. GET (BODY) - отправляет GET HTTP/S запрос по заданному адресу и сравнивает только возвращаемое тело ответа (Body) с заданным значением в поле Ответ (RAW).

         3. POST (CODE) - отправляет POST HTTP/S запрос по заданному адресу и сравнивает только возвращаемый ответ (RESPONSE STATUS. См. RFC 9110) с заданным значением в поле Ответ (RAW).

         4. POST (BODY) - отправляет POST HTTP/S запрос по заданному адресу и сравнивает только возвращаемое тело ответа (Body) с заданным значением в поле Ответ (RAW).

         5. ICMP - отправляет 32-битный пакет данных и в случае успешного ответа подтверждает доступность объекта. Эта технология может использоваться для проверки доступности разной внутренней инфраструктуры (компьютеры, роутеры и т. п.). Обратите внимание, что в поле HOST/IP адрес необходимо указать исключительно IP-адрес или имя хоста без приписок.

         6. TCP - производит подключение к указанному порту (по умолчанию порт - 80) и в случае установки соединения закрывает его, подтверждая доступность объекта. Обратите внимание, что в поле HOST/IP адрес необходимо указать исключительно IP-адрес или имя хоста без приписок. По умолчанию порт подключения - 80. Если необходимо указать другой порт, добавьте в конец адреса : и ваш порт. Например: google.com:80 или 8.8.8.8:53.

Время проверки (в сек.) - устанавливает интервал обращений к объекту для проверки его доступности.

Критический объект - устанавливает статус критического объекта (см. Система оценивания инфраструктуры).

Ответ (RAW) - поле для необходимого ответа на запрос. При отправке запроса, в зависимости от выбранного метода, программа получает ответ и сравнивает его с заданным. В случае их совпадения подтверждается доступность объекта.

ПРИМЕЧАНИЕ. В программе существует возможность автоматического получения ответа на запрос путем нажатия на кнопку Получить Ответ (RAW).

Получить Ответ (RAW) - отправляет запрос на указанный HOST/IP адрес и автоматически заносит полученный ответ в поле Ответ (RAW). При отправке запроса учитываются заданные данные (headers, body) и метод.

Настройка Headers (GET, POST) - позволяет настроить заголовки запроса, отправляемого объекту. Обратите внимание, что разделение заголовков происходит по символам ':' и ';'. Не используйте их в значениях.

Настройка Body (POST) - позволяет установить тело передаваемого POST запроса к объекту. Введенные данные передаются без изменений.

## Система оценивания инфраструктуры

В программе присутствует система оценивания общего состояния. Настроить ее можно на странице Настройки. Индикатор оценивания состояния находится на Главной странице в правом верхнем углу.

Режимы работы:

Только критические объекты (по умолчанию) - данный режим оценивает состояние инфраструктуры только основываясь на объектах, имеющих статус Критического объекта. Имеет два возможных индикатора: Красный и Зеленый. Если любой из Критических объектов становится недоступным, индикатор становится Красным. В остальных случаях он будет Зеленым.

Строгий - данный режим оценивает состояние инфраструктуры, учитывая все объекты вне зависимости от заданного статуса. Имеет два возможных индикатора: Красный и Зеленый. Если любой объект становится недоступным, индикатор становится Красным. В остальных случаях он будет Зеленым.

Кластерный + критические объекты - данный режим оценивает общую доступность инфраструктуры и Критические объекты. Имеет три возможных индикатора: Красный, Желтый и Зеленый. Если Критический объект становится недоступным, индикатор в любом случае становится Красным. Если общее количество доступных объектов менее 60%, индикатор становится Желтым, если менее 30% — Красным. В остальных случаях индикатор будет Зеленым. Этот режим может быть полезен при кластерной инфраструктуре. Например, можно установить Load Balancer как критический объект, а остальные web-серверы как обычные.

Отключено - данный режим отключает работу Системы оценивания инфраструктуры. Индикатор всегда будет серым.

## Система событий

В программе присутствует Система событий. Она предназначена для вывода событий, когда какой-то объект становится недоступным или доступным. Система всегда активна, и отключить ее нельзя. Открыть окно событий можно на Главной странице с помощью кнопки События. В случае появления новых событий рядом с кнопкой отобразится красный индикатор, сообщающий об этом.


