using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Temporary.Core
{
    public interface IDataWindowEntry
    {
        int id { get; }
        string displayName { get; }
        Sprite sprite { get; }
        void SetDisplayName(string name);
    }
}
