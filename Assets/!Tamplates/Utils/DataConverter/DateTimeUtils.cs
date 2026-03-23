using System;

public static class DateTimeUtils
{
    private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

    /// <summary>
    /// Конвертирует Unix timestamp (секунды с 01.01.1970) в DateTime
    /// </summary>
    /// <param name="timestamp">Unix timestamp в секундах</param>
    /// <returns>Локальная дата и время</returns>
    public static DateTime FromUnixTimestamp(long timestamp)
    {
        return UnixEpoch.AddSeconds(timestamp).ToLocalTime();
    }

    /// <summary>
    /// Конвертирует Unix timestamp (миллисекунды с 01.01.1970) в DateTime
    /// </summary>
    /// <param name="timestamp">Unix timestamp в миллисекундах</param>
    /// <returns>Локальная дата и время</returns>
    public static DateTime FromUnixTimestampMillis(long timestamp)
    {
        return UnixEpoch.AddMilliseconds(timestamp).ToLocalTime();
    }

    /// <summary>
    /// Возвращает количество секунд, прошедших с указанной даты до текущего момента
    /// (безопасная версия с проверкой переполнения)
    /// </summary>
    /// <param name="startDate">Начальная дата</param>
    /// <returns>Количество секунд (int), ограничено значениями int.MinValue/int.MaxValue</returns>
    public static int SecondsSince(DateTime startDate)
    {
        TimeSpan timePassed = DateTime.Now - startDate;
        double totalSeconds = timePassed.TotalSeconds;

        if (totalSeconds > int.MaxValue)
            return int.MaxValue;
        if (totalSeconds < int.MinValue)
            return int.MinValue;

        return (int)totalSeconds;
    }

    /// <summary>
    /// Возвращает количество секунд, прошедших с переданного Unix-времени до текущего момента
    /// </summary>
    public static int SecondsSinceUnixTimestamp(int unixTimestamp)
    {
        DateTime startDate = UnixEpoch.AddSeconds(unixTimestamp);
        TimeSpan timePassed = DateTime.UtcNow - startDate;

        double totalSeconds = timePassed.TotalSeconds;

        if (totalSeconds > int.MaxValue)
            return int.MaxValue;
        if (totalSeconds < int.MinValue)
            return int.MinValue;

        return (int)totalSeconds;
    }

    /// <summary>
    /// Получает текущую дату/время в Unix timestamp (секундах с 01.01.1970)
    /// </summary>
    /// <returns>Количество секунд с Unix epoch</returns>
    public static long GetCurrentUnixTimestamp()
    {
        return (long)(DateTime.UtcNow - UnixEpoch).TotalSeconds;
    }
}