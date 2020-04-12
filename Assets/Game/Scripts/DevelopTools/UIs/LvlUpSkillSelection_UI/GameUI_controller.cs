﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tools.Extensions;
using UnityEngine.UI;

public class GameUI_controller : MonoBehaviour
{
    public static GameUI_controller instance;
    [SerializeField] Canvas myCanvas; public Canvas MyCanvas { get => myCanvas; }

    [SerializeField] private GameObject skillSelection_template_pf;
    [SerializeField] private GameObject charStats_template_pf;
    [SerializeField] private WorldCanvasPopUp lvlUp_pf;//quiero sacarlo de aca. Habria que hacer un pool o biblioteca de feedbacks
    [SerializeField] private GameMenu_UI gameMenu_UI;

    private CharStats_UI _charStats_Ui;
    Dictionary<UI_templates, GameObject> UiTemplateRegistry = new Dictionary<UI_templates, GameObject>();

    [Header("--XX--Canvas containers--XX--")]
    [SerializeField] private RectTransform leftCanvas;
    [SerializeField] private RectTransform rightCanvas;

    private SkillManager_Pasivas _skillManagerPasivas;

    public bool openUI { get; private set; }


    #region Config

    void Awake()
    {
        if (instance == null)
            instance = this;

        RegistrarUIPrefabs();
    }

    public void Initialize()
    {
        _charStats_Ui = Instantiate<GameObject>(UiTemplateRegistry[UI_templates.charStats], leftCanvas).GetComponent<CharStats_UI>();
        gameMenu_UI.Configure(_charStats_Ui.ToggleLvlUpSignOFF);
    }

    private void RegistrarUIPrefabs()
    {
        UiTemplateRegistry.Add(UI_templates.skillSelection, skillSelection_template_pf);
        UiTemplateRegistry.Add(UI_templates.charStats, charStats_template_pf);
    }

    #endregion

    #region Public methods
    /// <summary>
    /// DEPRECATED
    /// </summary>
    /// <param name="uiTemplates"></param>
    /// <returns></returns>
    public LvlUpSkillSelection_UI CreateNewSkillSelectionPopUp(List<SkillInfo> skillsParaElegir, Action<SkillInfo> callback)
    {

        GameObject go = new GameObject();
        LvlUpSkillSelection_UI newPopUp = Instantiate(UiTemplateRegistry[UI_templates.skillSelection]).GetComponent<LvlUpSkillSelection_UI>();
        newPopUp.Configure(skillsParaElegir, callback, () => Debug.Log("esto se esta usando?"), out go);
        return newPopUp;
    }
    public void UI_Send_NameSkillType(string s) { }
    public void UI_SendLevelUpNotification() => CanvasPopUpInWorld_Manager.instance.MakePopUpAnimated(Main.instance.GetChar().transform, lvlUp_pf);
    public void UI_SendActivePlusNotification(bool val) { if (val) _charStats_Ui.ToggleLvlUpSignON(); }
    public void UI_RefreshExpBar(int currentExp, int maxExp, int currentLevel) => _charStats_Ui.UpdateXP_UI(currentExp, maxExp, currentLevel);
    public void RefreshPassiveSkills_UI(List<SkillInfo> skillsNuevas) => _charStats_Ui.UpdatePasiveSkills(skillsNuevas);
    public void SetSelectedPath(string pathName) => _charStats_Ui.SetPathChoosen(pathName);
    public void UI_RefreshMenu() => gameMenu_UI.Refresh();
    public void OpenGameMenu() => gameMenu_UI.Open();
    public void CloseGameMenu() => gameMenu_UI.Close();
    #endregion
    public void Set_Opened_UI() { openUI = true; Main.instance.Pause(); }
    public void Set_Closed_UI() { openUI = false; Main.instance.Play(); }
    public void BTN_Back_OpenMenu()
    {
        if (!openUI)
        {
            OpenGameMenu();
            Set_Opened_UI();
        }
        else
        {
            CloseGameMenu();
            Set_Closed_UI();
        }
    }
}
