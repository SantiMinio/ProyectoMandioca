﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UI_CurrentItem : UI_Base
{
    [SerializeField] TextMeshProUGUI txt_cant;
    [SerializeField] Image img;

    public void SetItem(string _cant, Sprite _img)
    {
        txt_cant.text = _cant;
        img.sprite = _img;
    }

    #region unused
    public override void Refresh() { }
    protected override void OnAwake() { }
    protected override void OnEndCloseAnimation() { }
    protected override void OnEndOpenAnimation() { }
    protected override void OnStart() { }
    protected override void OnUpdate() { }
    #endregion
}
