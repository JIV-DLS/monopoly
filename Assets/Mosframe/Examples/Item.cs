﻿/**
 * item.cs
 * 
 * @author mosframe / https://github.com/mosframe
 * 
 */

namespace Mosframe {

    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;

    public class Item : UIBehaviour, IDynamicScrollViewItem 
    {
	    private readonly Color[] colors = new Color[] {
		    Color.cyan,
		    Color.green,
	    };

	    public Image icon;
	    public Text  title;
	    public Image background;

        public void onUpdateItem( int index ) {

		    this.title.text         = string.Format("Name{0:d3}", (index + 1) );
		    this.background.color   = this.colors[Mathf.Abs(index) % this.colors.Length];
		    this.icon.sprite        = Resources.Load<Sprite>( (Mathf.Abs(index) % 20 + 1).ToString("icon_00") );
        }
    }
}