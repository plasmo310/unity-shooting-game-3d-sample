using UnityEngine;
using UnityEngine.Events;

namespace Services
{
    public interface IDialogService
    {
        public void ShowNormalDialog(string text, UnityAction callback);
        public void HideNormalDialog();
    }
}
