using System.ComponentModel.DataAnnotations;

namespace brk.Framework.Utility.Enums;

[Flags]
public enum DayOfWeekPersian
{
    [Display(Name = "شنبه")]
    Saturday = 1,
    
    [Display(Name = "یکشنبه")]
    Sunday = 2,
    
    [Display(Name = "دوشنبه")]
    Monday = 4,
    
    [Display(Name = "سه شنبه")]
    Tuesday = 8,
    
    [Display(Name = "چهارشنبه")]
    Wednesday = 16,
    
    [Display(Name = "پنج شنبه")]
    Thursday = 32,
    
    [Display(Name = "جمعه")]
    Friday = 64,
}