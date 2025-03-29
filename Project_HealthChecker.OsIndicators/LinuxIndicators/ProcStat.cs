using System.Text.RegularExpressions;

namespace Project_HealthChecker.OsIndicators.LinuxIndicators;

/// <summary>
/// Структура данных о ядре (в файле /proc/stat).
/// </summary>
public struct ProcStat
{
    /// <summary>
    /// Номер ядра процессора, включая Hyper-Threading (от 0).
    /// </summary>
    public string CpuNumber;
    
    /// <summary>
    /// Время, проведённое в пользовательском режиме (без учета процессов с низким приоритетом), в единицах, зависящих от системы.
    /// </summary>
    public int User;

    /// <summary>
    /// Время, проведённое в пользовательском режиме с низким приоритетом (nice).
    /// </summary>
    public int Nice;

    /// <summary>
    /// Время, проведённое в режиме ядра.
    /// </summary>
    public int System;

    /// <summary>
    /// Время простоя, когда система не выполняла никаких задач.
    /// </summary>
    public int Idle;

    /// <summary>
    /// Время ожидания ввода-вывода — период, когда процессор простаивал, ожидая завершения операций ввода-вывода.
    /// </summary>
    public int Iowait;

    /// <summary>
    /// Время, затраченное на обслуживание аппаратных прерываний.
    /// </summary>
    public int Irq;

    /// <summary>
    /// Время, затраченное на обслуживание программных прерываний.
    /// </summary>
    public int Softirq;

    /// <summary>
    /// Время, в течение которого виртуальный процессор ожидал реального процессора, будучи "украденным" гипервизором (актуально для виртуальных машин).
    /// </summary>
    public int Steal;

    /// <summary>
    /// Время, проведённое на выполнение гостевых (виртуальных) процессов под управлением гипервизора.
    /// </summary>
    public int Guest;

    /// <summary>
    /// Время, проведённое на выполнение гостевых процессов с низким приоритетом.
    /// </summary>
    public int GuestNice;

    /// <summary>
    /// Сформировать данные о ядре из строки.
    /// </summary>
    /// <param name="str">строка.</param>
    /// <returns>Структура данных о ядре.</returns>
    public static ProcStat FromString(string str)
    {
        var stringSegments = Regex
            .Replace(str, @"\s{2,}", " ")
            .Split(' ');

        return new ProcStat
        {
            CpuNumber = stringSegments[0],
            User = int.Parse(stringSegments[1]),
            Nice = int.Parse(stringSegments[2]),
            System = int.Parse(stringSegments[3]),
            Idle = int.Parse(stringSegments[4]),
            Iowait = int.Parse(stringSegments[5]),
            Irq = int.Parse(stringSegments[6]),
            Softirq = int.Parse(stringSegments[7]),
            Steal = int.Parse(stringSegments[8]),
            Guest = int.Parse(stringSegments[9]),
            GuestNice = int.Parse(stringSegments[10])
        };
    }
}