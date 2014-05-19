using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DPMiner
{
    public enum Stages:byte
    { begining, dataVault, logic, mining, model}
    public interface IMiningState:IDisposable
    {
        /// <summary>
        /// Возвращает следующий этап анализа
        /// </summary>
        /// <returns></returns>
        IMiningState Next();
        /// <summary>
        /// Возвращает элементы управления для работы на текущем этапе анализа
        /// </summary>
        /// <returns>элементы управления для работы на текущем этапе анализа</returns>
        Control Handle();
        bool IsEnd();
        bool HideControls();
        Stages Stage();
        
    }                                                                                                                             
}
