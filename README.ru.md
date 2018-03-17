# ModAlarm
Серверный модуль для [Rapid SCADA](https://rapidscada.ru/).  
Позволяет выводить звук аварийной сигнализации.

Звук воспроизводится только на сервере.

Проверено на Windows 7/10. Linux не поддерживается.

### Установка
Распаковать архив в каталог SCADA\ScadaServer.  
Запустить ScadaServerCtrl.exe.  
Включить ModAlarm на вкладке модули.

### Скриншот
![ModAlarm screenshot](https://github.com/kolod/modalarm/raw/master/screenshot.ru.png)

### Примечания
Для работы модуля, необходимо наличие хотя бы одного канала с типом "Дорасчётный ТИ" в проекте.

### Пример использования:
![Administrator screenshot](https://github.com/kolod/modalarm/raw/master/screenshot-setup.ru.png)

```C#
// Формула для канала 65000
public double Blink() {
  DateTime time = DateTime.Now;
  if ((time.Second % 2) > 0) {
    return 0.0;
  } else {
    return 1.0;
  }
}

// Формула для канала 65001
public double GetTime() {
  return EncodeDate(DateTime.Now);
}

// Формула для канала 65002
public double GetAllAlarms(int first, int last) {
  UInt64 bitmask = 0;
  const UInt64 one = 1;
  int count = last-first+1;
  for (int i = 0; i < count; i++) {
    int chanel = (first + i) * 100 + 11;
    if (Val(chanel) > 1.5) {   // 0 - готовность; 1 - работа; 2 - нет связи; 3 - авария
      bitmask |= (one << chanel);
    }
  }
  return bitmask;
}

// Формула для канала 65003
public double GetNewAlarms() {
  UInt64 alarms     = (UInt64) Val(65002); // Текущие аварии
  UInt64 old_alarms = (UInt64) Val(65006); // Аварии на прошлом цикле
  return (alarms & ~old_alarms);
}

// Формула для канала 65004
public double UnCheckBackAlarm() {
  UInt64 new_alarms     = (UInt64) Val(65003); // Новые аварии
  UInt64 checked_alarms = (UInt64) Val(65004); // Квитированные аварии
  checked_alarms &= ~new_alarms;               // Флаг квитирования сбрасывается при отсутствии аварии
  return checked_alarms;
}

// Формула для канала 65005
public double GetUnchekedAlarms() {
  UInt64 alarms         = (UInt64) Val(65002);
  UInt64 checked_alarms = (UInt64) Val(65004);
  UInt64 value          = alarms & ~checked_alarms;
  return value;
}

// Формула для канала 65007
public double IndicateAlarm() {
  UInt64 alarm = (UInt64) Val(65005); // Неквитированные аварии
  if (alarm > 0) {
    bool blink = Val(65000) > 0.5;
    if (blink) {
      return 1.0;
    } else {
      return 2.0;
    }
  }

  return 0.0;
}

// Формула для канала комманд №65004
double CheckBackAlarm() {
  UInt64 value = (UInt64) Val(65002); // Все текущие аварии
  SetVal(CnlNum, value);
  return value;
}
```