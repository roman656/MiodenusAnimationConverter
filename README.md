# MiodenusAnimationConverter

*MiodenusAnimationConverter* - это консольное приложение, входящее в состав *Miodenus Project*, отвечающее за преобразование входных данных ([см. ниже](#входные-данные)) в итоговый видеофайл, содержащий анимацию сборки.

## Демо

Ниже представлена демонстрационная сборка редуктора привода лебёдки, созданная в *MiodenusAnimationConverter*.

<p align="center"><img src="https://user-images.githubusercontent.com/57591626/162587743-1500fce5-f069-4e37-bf47-8021115e6a65.gif" alt="Демо" /></p>

## Входные данные

На вход данное приложение принимает файл, описывающий анимацию сборки. Его структура представлена ниже.

### Анимационный файл

Ниже представлен пример содержимого анимационного файла.

```json
{
  "animationInfo":
  {
    "type": "maf",
    "version": "1.0",
    "name": "Demo assembling",
    "videoFormat": "mp4",
    "videoCodec": "mpeg4",
    "videoBitrate": 4000,
    "videoName": "ResultVideo",
    "timeLength": 0,
    "fps": 60,
    "enableMultisampling": true,
    "frameWidth": 800,
    "frameHeight": 800,
    "backgroundColor": [0.3, 0.3, 0.4],
    "include": [ "Demo/Includes/header.maf" ]
  },
  "modelsInfo":
  [
    {
      "name": "00_000_06_13_13_01",
      "type": "stl",
      "filename": "Demo/Models/00_000_06_13_13_01.stl",
      "color": [0.45, 0.52, 0.58],
      "useCalculatedNormals": false
    },
    {
      "name": "00_000_06_13_13_02",
      "type": "stl",
      "filename": "Demo/Models/00_000_06_13_13_02.stl",
      "color": [0.45, 0.52, 0.58],
      "useCalculatedNormals": false
    },
    { "name": "00_000_06_13_13_03", "filename": "Demo/Models/00_000_06_13_13_03.stl" },
    { "name": "00_000_06_13_13_04", "filename": "Demo/Models/00_000_06_13_13_04.stl" },
    { "name": "00_000_06_13_13_05", "filename": "Demo/Models/00_000_06_13_13_05.stl" },
    { "name": "00_000_06_13_13_06", "filename": "Demo/Models/00_000_06_13_13_06.stl" },
    { "name": "00_000_06_13_13_07", "filename": "Demo/Models/00_000_06_13_13_07.stl" },
    { "name": "00_000_06_13_13_08", "filename": "Demo/Models/00_000_06_13_13_08.stl" },
    { "name": "00_000_06_13_13_09", "filename": "Demo/Models/00_000_06_13_13_09.stl" },
    { "name": "00_000_06_13_13_10", "filename": "Demo/Models/00_000_06_13_13_10.stl" },
    { "name": "00_000_06_13_13_11_1", "filename": "Demo/Models/00_000_06_13_13_11.stl" },
    { "name": "00_000_06_13_13_11_2", "filename": "Demo/Models/00_000_06_13_13_11.stl" },
    { "name": "00_000_06_13_13_12", "filename": "Demo/Models/00_000_06_13_13_12.stl" },
    { "name": "00_000_06_13_13_13", "filename": "Demo/Models/00_000_06_13_13_13.stl" },
    { "name": "00_000_06_13_13_14", "filename": "Demo/Models/00_000_06_13_13_14.stl" },
    { "name": "00_000_06_13_13_16", "filename": "Demo/Models/00_000_06_13_13_16.stl", "color": [0.15, 0.15, 0.15] },
    { "name": "00_000_06_13_13_19", "filename": "Demo/Models/00_000_06_13_13_19.stl" }
  ],
  "bindings":
  [
    {
      "modelName": "00_000_06_13_13_01",
      "actionName": "Init 01",
      "startTime": 0,
      "timeLength": 0,
      "useInterpolation": false
    },
    {
      "modelName": "00_000_06_13_13_02",
      "actionName": "Init 02",
      "startTime": 0,
      "timeLength": 0,
      "useInterpolation": false
    },
    { "modelName": "00_000_06_13_13_03", "actionName": "Init 03", "useInterpolation": false },
    { "modelName": "00_000_06_13_13_04", "actionName": "Init 04", "useInterpolation": false },
    { "modelName": "00_000_06_13_13_05", "actionName": "Init 05", "useInterpolation": false },
    { "modelName": "00_000_06_13_13_06", "actionName": "Init 06", "useInterpolation": false },
    { "modelName": "00_000_06_13_13_07", "actionName": "Init 07", "useInterpolation": false },
    { "modelName": "00_000_06_13_13_08", "actionName": "Init 08", "useInterpolation": false },
    { "modelName": "00_000_06_13_13_09", "actionName": "Init 09", "useInterpolation": false },
    { "modelName": "00_000_06_13_13_10", "actionName": "Init 10", "useInterpolation": false },
    { "modelName": "00_000_06_13_13_11_1", "actionName": "Init 11-1", "useInterpolation": false },
    { "modelName": "00_000_06_13_13_11_2", "actionName": "Init 11-2", "useInterpolation": false },
    { "modelName": "00_000_06_13_13_12", "actionName": "Init 12", "useInterpolation": false },
    { "modelName": "00_000_06_13_13_13", "actionName": "Init 13", "useInterpolation": false },
    { "modelName": "00_000_06_13_13_14", "actionName": "Init 14", "useInterpolation": false },
    { "modelName": "00_000_06_13_13_16", "actionName": "Init 16", "useInterpolation": false },
    { "modelName": "00_000_06_13_13_19", "actionName": "Init 19", "useInterpolation": false },
    { "modelName": "00_000_06_13_13_01", "actionName": "Переместить 01 в центр", "startTime": 1000 },
    { "modelName": "00_000_06_13_13_05", "actionName": "Переместить 05 в центр", "startTime": 1500 },
    { "modelName": "00_000_06_13_13_19", "actionName": "Соединить 19 и 05", "startTime": 2500 },
    { "modelName": "00_000_06_13_13_07", "actionName": "Соединить 07 и 05", "startTime": 3500 },
    { "modelName": "00_000_06_13_13_16", "actionName": "Соединить 16 и 05", "startTime": 4500 },
    { "modelName": "00_000_06_13_13_04", "actionName": "Переместить 04 в центр", "startTime": 5500 },
    { "modelName": "00_000_06_13_13_03", "actionName": "Соединить 04 и 03", "startTime": 6500 },
    { "modelName": "00_000_06_13_13_03", "actionName": "Соединить 04 + 03 и 05", "startTime": 9000 },
    { "modelName": "00_000_06_13_13_04", "actionName": "Соединить 04 + 03 и 05", "startTime": 9000 },
    { "modelName": "00_000_06_13_13_10", "actionName": "Переместить 10 к сборке", "startTime": 10000 },
    { "modelName": "00_000_06_13_13_09", "actionName": "Соединить 09 и 10", "startTime": 10500 },
    { "modelName": "00_000_06_13_13_13", "actionName": "Соединить 13 и 10", "startTime": 11000 },
    { "modelName": "00_000_06_13_13_14", "actionName": "Соединить 14 и 10", "startTime": 12000 },
    { "modelName": "00_000_06_13_13_10", "actionName": "Соединить 10 + 09 + 13 + 14 и 01", "startTime": 14500 },
    { "modelName": "00_000_06_13_13_09", "actionName": "Соединить 10 + 09 + 13 + 14 и 01", "startTime": 14500 },
    { "modelName": "00_000_06_13_13_13", "actionName": "Соединить 10 + 09 + 13 + 14 и 01", "startTime": 14500 },
    { "modelName": "00_000_06_13_13_14", "actionName": "Соединить 10 + 09 + 13 + 14 и 01", "startTime": 14500 },
    { "modelName": "00_000_06_13_13_06", "actionName": "Соединить 06 и 10", "startTime": 15500 },
    { "modelName": "00_000_06_13_13_11_1", "actionName": "Соединить 11 и 10", "startTime": 16500 },
    { "modelName": "00_000_06_13_13_11_2", "actionName": "Соединить 11 и 10", "startTime": 16500 },
    { "modelName": "00_000_06_13_13_12", "actionName": "Соединить 12 и два 11", "startTime": 18500 },
    { "modelName": "00_000_06_13_13_02", "actionName": "Переместить 02 в центр", "startTime": 22000 },
    { "modelName": "00_000_06_13_13_08", "actionName": "Соединить 08 и 02", "startTime": 23000 },
    { "modelName": "00_000_06_13_13_02", "actionName": "Поднять 08 и 02", "startTime": 26000 },
    { "modelName": "00_000_06_13_13_08", "actionName": "Поднять 08 и 02", "startTime": 26000 },
    { "modelName": "00_000_06_13_13_02", "actionName": "Скрыть нижнюю крышку", "startTime": 27500, "useInterpolation": false },
    { "modelName": "00_000_06_13_13_08", "actionName": "Скрыть нижнюю крышку", "startTime": 27500, "useInterpolation": false },
    { "modelName": "00_000_06_13_13_04", "actionName": "Вращать 04", "startTime": 29000 },
    { "modelName": "00_000_06_13_13_05", "actionName": "Вращать 04", "startTime": 29000 },
    { "modelName": "00_000_06_13_13_03", "actionName": "Вращать 04", "startTime": 29000 },
    { "modelName": "00_000_06_13_13_06", "actionName": "Вращать 06", "startTime": 29000 },
    { "modelName": "00_000_06_13_13_10", "actionName": "Вращать 06", "startTime": 29000 },
    { "modelName": "00_000_06_13_13_11_1", "actionName": "Вращать 06", "startTime": 29000 },
    { "modelName": "00_000_06_13_13_11_2", "actionName": "Вращать 06", "startTime": 29000 }
  ]
}
```

## Выходные данные

В результате работы приложения будет получен видеофайл, содержащий анимацию сборки. В случае передачи особых аргументов результат работы приложения может быть другим ([см. ниже](#таблица-аргументов)).

## Запуск из консоли
### Таблица аргументов

| Обязательный аргумент | Ключ | Значение | Примечание |
| :---: | :---: | :---: | --- |
| Да | `-a` `--animation` | Путь до файла, описывающего анимацию сборки | Путь лучше заключать в двойные `"` кавычки. |
| Нет | `-q` `--quiet` | Отсутствует | В данном режиме приложение не будет выводить сообщения (предупреждения, ошибки...) в консоль. Вывод информации в журнал не будет остановлен. |
| Нет | `-v` `--view` | Номер кадра, который будет загружен для просмотра `*` | Будет запущено графическое окно (размер и другие параметры берутся из файла, описывающего анимацию сборки) в котором будет преставлено состояние анимации (сцена, положение камер...) на момент указанного кадра. При помощи определенных комбинаций клавиш (см. ниже) пользователь сможет осмотреть сцену. Если значение аргумента равно `0` — будет показана вся анимация. В данном режиме приложение не будет производить запись анимации. Диапазон допустимых значений: от `0` до `числа кадров в итоговом видео сборки`. |
| Нет | `-f` `--frame` | Номер кадра, который требуется получить в виде файла изображения `*` | В папке `screenshots` появится файл изображения указанного кадра. Наименование файла будет иметь следующий вид: `<номер-кадра>_<дата>.png`, где `<дата>` имеет формат `ГГГГ_ММ_ДД_чч_мм_сс`. В случае если файл с таким именем уже существует — он будет перезаписан. В данном режиме приложение не будет производить запись анимации. Диапазон допустимых значений: от `1` до `числа кадров в итоговом видео сборки`. |
| Нет | `--help` | Отсутствует | В консоль будет выведена подробная справка о работе программы, после чего приложение завершит свою работу. |
| Нет | `--version` | Отсутствует | В консоль будет выведена информация о версии установленной программы, после чего приложение завершит свою работу. |

`*` — *Аргументы `--view` и `--frame` не могут применяться одновременно!*

### Пример запуска из консоли

Команда, представленная ниже, создаст итоговый видеофайл сборки, используя анимационный файл *animation.maf*. Приложение не будет выводить какие-либо сообщения в консоль.

`$ ./MiodenusAnimationConverter --animation "path/to/animation.maf" --quiet`

## Установка

Можно воспользоваться уже [скомпилированными исходниками](https://cloud.mail.ru/public/WjBw/uoZge8X91) (Windows 10 / Kubuntu 21.10).

Так же можно воспользоваться [MiodenusUI](https://github.com/PoorMercymain/MiodenusUI) для работы над файлом анимации постредством графического интерфейса (GUI).

### Особенности установки на Windows

В директорию проекта (где находятся файлы с расширением .dll) необходимо поместить файл *ffmpeg.exe*.

### Особенности установки на Linux

Приложение было протестировано под ОС [Kubuntu 21.10](https://kubuntu.org/).

Рекомендуется использовать проприетарные драйвера для видеокарты.

Установка [.NET 5](https://docs.microsoft.com/ru-ru/dotnet/core/install/linux-ubuntu) в Kubuntu.

Установка зависимостей:

`sudo apt install -y libgdiplus`

`sudo apt install ffmpeg`
