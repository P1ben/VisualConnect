using MaterialDesignColors.Recommended;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using Viscon.Model.Connectables;

namespace Viscon.UserControls
{
    public interface ComInterface
    {
        object getParent();
        double GetPosX();
        double GetPosY();
        long GetModelId();
        List<(Border border, Connectable connectable, ConnectionType conn_type)> GetParamsMap();
    }
}
