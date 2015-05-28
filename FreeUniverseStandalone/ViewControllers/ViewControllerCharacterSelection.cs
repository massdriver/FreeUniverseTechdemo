using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using FreeUniverse.GUILib;

namespace FreeUniverse.ViewControllers
{
    public class ViewControllerCharacterSelection : ViewControllerBase
    {
        public ViewControllerCharacterSelection()
        {
            visible = false;

            tableView = new TableView(Rect.MinMaxRect(0.0f, 0.0f, TABLE_VIEW_WIDTH - TABLE_OFFSET, TABLE_VIEW_HEIGHT / 2.0f - 2.0f * TABLE_VERTICAL_OFFSET));

        }

        public IViewControllerCharacterSelectionDelegate controllerDelegate { get; set; }

        private const float TABLE_VERTICAL_OFFSET = 20.0f;
        private const float TABLE_OFFSET = 10.0f;
        private const float TABLE_VIEW_WIDTH = 300.0f;
        private const float TABLE_VIEW_HEIGHT = 400.0f;

        public override void OnGUI(UnityEngine.Event e)
        {
            base.OnGUI(e);

            if (!visible)
                return;

            GUI.Window(GUIStaticData.WND_CHARACTERSELECTION_TABLE, Helpers.MakeRectCentered(TABLE_VIEW_WIDTH, TABLE_VIEW_HEIGHT), CharacterTableView, "Characters");
        }

        private TableView tableView { get; set; }

        void CharacterTableView(int id)
        {
            GUILayout.BeginArea(Rect.MinMaxRect(TABLE_OFFSET, TABLE_VERTICAL_OFFSET, TABLE_VIEW_WIDTH - TABLE_OFFSET, TABLE_VIEW_HEIGHT - TABLE_VERTICAL_OFFSET));
            tableView.OnGUI();
            GUILayout.Space(10.0f);

            if (GUILayout.Button("Log Selected Character") && controllerDelegate != null)
                controllerDelegate.OnCharacterSelectionViewControllerLogCharacter(this, tableView.selectedRow);

            GUILayout.Space(10.0f);

            if (GUILayout.Button("Create New Character") && controllerDelegate != null)
                controllerDelegate.OnCharacterSelectionViewControllerCreateCharacter(this);

            if (GUILayout.Button("Delete Character") && controllerDelegate != null)
                controllerDelegate.OnCharacterSelectionViewControllerDeleteCharacter(this, tableView.selectedRow);

            GUILayout.EndArea();

            if (Input.GetKeyDown(KeyCode.Escape) && controllerDelegate != null)
                controllerDelegate.OnCharacterSelectionViewControllerBack(this);
        }
    }
}
