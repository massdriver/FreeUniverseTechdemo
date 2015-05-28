using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using FreeUniverse.GUILib;

namespace FreeUniverse.ViewControllers
{
    public interface IViewControllerCharacterSelectionDelegate
    {
        void OnCharacterSelectionViewControllerCreateCharacter(ViewControllerCharacterSelection controller);
        void OnCharacterSelectionViewControllerDeleteCharacter(ViewControllerCharacterSelection controller, int id);
        void OnCharacterSelectionViewControllerLogCharacter(ViewControllerCharacterSelection controller, int id);
        void OnCharacterSelectionViewControllerBack(ViewControllerCharacterSelection controller);
    }
}