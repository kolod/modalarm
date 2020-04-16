# ModAlarm
Server module for the [Rapid SCADA](https://rapidscada.org/).  
Allows you to output the alarm sound.

The alarm sound plays only on the server.

Tested on Windows 7/8.1/10.
Linux not supported.

### Install
Unpack the archive in the SCADA\ScadaServer directory.  
Run ScadaServerCtrl.exe.  
Enable ModAlarm on the modules tab.

### Screenshot
![ModAlarm screenshot](https://github.com/kolod/modalarm/raw/master/screenshot.png)

### Notes
For the module to work, it is necessary to have at least one channel with the type "Calculated real" in the project.
Please check that ModAlert.dll and NAudio.dll aren't locked.
![ModAlarm unlock screenshot](https://github.com/kolod/modalarm/raw/master/screenshot-unlock.png)

### Usage example:
![Administrator screenshot](https://github.com/kolod/modalarm/raw/master/screenshot-setup.png)

```C#
// Formula for channel 65000
public double Blink() {
  DateTime time = DateTime.Now;
  if ((time.Second % 2) > 0) {
    return 0.0;
  } else {
    return 1.0;
  }
}

// Formula for channel 65001
public double GetTime() {
  return EncodeDate(DateTime.Now);
}

// Formula for channel 65002
public double GetAllAlarms(int first, int last) {
  UInt64 bitmask = 0;
  const UInt64 one = 1;
  int count = last-first+1;
  for (int i = 0; i < count; i++) {
    int chanel = (first + i) * 100 + 11;
    if (Val(chanel) > 1.5) {   // 0 - ready; 1 - run; 2 - communication lost; 3 - alarm
      bitmask |= (one << chanel);
    }
  }
  return bitmask;
}

// Formula for channel 65003
public double GetNewAlarms() {
  UInt64 alarms     = (UInt64) Val(65002); // All current alarms
  UInt64 old_alarms = (UInt64) Val(65006); // All alarms on the last cycle
  return (alarms & ~old_alarms);
}

// Formula for channel 65004
public double UnCheckBackAlarm() {
  UInt64 new_alarms     = (UInt64) Val(65003); // New alarms
  UInt64 checked_alarms = (UInt64) Val(65004); // Acknowledged alarms
  checked_alarms &= ~new_alarms;               // The acknowledgment flag is resetting in the absence of an alarm
  return checked_alarms;
}

// Formula for channel 65005
public double GetUnchekedAlarms() {
  UInt64 alarms         = (UInt64) Val(65002);
  UInt64 checked_alarms = (UInt64) Val(65004);
  UInt64 value          = alarms & ~checked_alarms;
  return value;
}

// Formula for channel 65007
public double IndicateAlarm() {
  UInt64 alarm = (UInt64) Val(65005); // Non-acknowledged alarms
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

// Formula for the command channel 65004
double CheckBackAlarm() {
  UInt64 value = (UInt64) Val(65002); // All current alarms
  SetVal(CnlNum, value);
  return value;
}
```