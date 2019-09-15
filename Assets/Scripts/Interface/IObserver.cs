using System.Collections.Generic;
using UnityEngine;

public interface IObserver
{
    void UpdateUserData(UserDataIO.User user);

    void UpdateFieldData(Dictionary<string, List<GameObject>> objects);
}