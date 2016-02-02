using System;

namespace justinobney.gymbuddy.api.Interfaces
{
    public interface IHasTouchedProperties
    {
        DateTime? CreatedAt { get; set; }
        DateTime? ModifiedAt { get; set; }
    }
}