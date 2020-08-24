using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Menus
{
    public interface IAmClickable
    {
        public Action OnClick { get; set; }
    }
}
