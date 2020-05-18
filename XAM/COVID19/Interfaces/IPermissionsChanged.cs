using Perimeter.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Perimeter.Interfaces
{
    public interface IPermissionsChanged
    {
        void permissionsChanged(GeneralPermission[] changedPermissions);
    }
}
