// using UnityEngine;
// using System.Collections;
// using UnityEditor;
// using Skill.Editor.UI;
// using Skill.Editor.Curve;
// using Skill.Framework.UI;

// [CustomEditor(typeof(UpgradeManager))]
// public class GraphEditor : EditorWindow
// {
// 	private static Vector2 Size = new Vector2(800, 600);
// 	private static GraphEditor _Instance;

// 	public static GraphEditor Instance
// 	{
// 		get
// 		{
// 			if (_Instance == null)
// 				_Instance = ScriptableObject.CreateInstance<GraphEditor>();
// 			return _Instance;
// 		}
// 	}

// 	void OnDestroy()
// 	{
// 		_Instance = null;
// 	}
// 	void OnFocus()
// 	{
// 		if (_Frame != null)
// 		{
// 			Rebuild();
// 		}
// 	}
	
// 	private void Rebuild()
// 	{
// 		//_CurveEditor.Selection.Clear();
// 		//_CurveEditor.RemoveAllCurves();
// 		// if (_Object != null)
// 		// {
// 		// 	Component[] components = _Object.GetComponents<Component>();
// 		// 	foreach (var c in components)
// 		// 	{
// 		// 		AddCurves(c);
// 		// 	}
// 		// }
// 	}
		
// 	private EditorFrame _Frame;
// 	private CurveEditor _CurveEditor;
// 	private Skill.Framework.UI.Grid _PnlLeft;
// 	private TreeView _CurveTreeView;
// 	private Skill.Editor.UI.GridSplitter _GridSplitter;
	
// 	private void CreateUI()
// 	{
// 		_Frame = new EditorFrame("Frame", this);

// 		_Frame.Grid.ColumnDefinitions.Add(224, Skill.Framework.UI.GridUnitType.Pixel);// _PnlLeft
// 		_Frame.Grid.ColumnDefinitions[0].MinWidth = 224;
// 		_Frame.Grid.ColumnDefinitions.Add(2, Skill.Framework.UI.GridUnitType.Pixel); // _GridSplitter
// 		_Frame.Grid.ColumnDefinitions.Add(5, Skill.Framework.UI.GridUnitType.Star);  // _CurveEditor        

// 		_PnlLeft = new Skill.Framework.UI.Grid() { Row = 0, Column = 0 };
// 		_PnlLeft.RowDefinitions.Add(1, Skill.Framework.UI.GridUnitType.Star); // _CurveTreeView        

// 		_CurveTreeView = new TreeView() { Row = 1, Column = 0 };
// 		_CurveTreeView.DisableFocusable();

// 		_PnlLeft.Controls.Add(_CurveTreeView);


// 		_GridSplitter = new Skill.Editor.UI.GridSplitter() { Row = 0, Column = 1, Orientation = Skill.Framework.UI.Orientation.Vertical };
// 		_CurveEditor = new CurveEditor() { Row = 0, Column = 2};

// 		_Frame.Controls.Add(_PnlLeft);
// 		_Frame.Controls.Add(_GridSplitter);
// 		_Frame.Controls.Add(_CurveEditor);
		
// 		AddTestCurves();
// 	}
	
// 	private void AddTestCurves()
// 	{
// 		UpgradeManager manager = GameObject.Find("UpgradeManager").GetComponent<UpgradeManager>();
// 		FunctionTrack sum = new FunctionTrack((TimeLineCurveView)_CurveEditor.TimeLine.View, ((float level)=>{ return manager.EnemyHP.Evaluate(level);}));
		
// 		_CurveEditor.TimeLine.View.Controls.Add(sum);
// 		_CurveEditor.TimeLine.View.Controls.Add(new FunctionTrack((TimeLineCurveView)_CurveEditor.TimeLine.View,
// 		((float level)=>
// 		{
// 			float totalGold = manager.Reward.Evaluate(level);
// 			float damage = 3.0f;
// 			if(manager.Price.length > 0)
// 			{
// 				for(int i = 0, c = (int)manager.Price.keys[manager.Price.length - 1].time; i < c; ++i)
// 				{
// 					if(manager.Price.Evaluate(i) > totalGold)
// 					{
// 						break;
// 					}
// 					damage = manager.Damage.Evaluate(i) / 3.0f;
// 					if(i > 0) damage += manager.Damage.Evaluate(i - 1) / 3.0f;
// 					if(i > 1) damage += manager.Damage.Evaluate(i - 2) / 3.0f;
					
// 				}
// 			}
// 			return damage;
// 		} )));
		
// 		CurveTrack track = _CurveEditor.AddCurve(manager.EnemyHP, Color.green);
// 		track.Changed += sum.CurveChange;
// 		_CurveTreeView.Controls.Add(new CurveTrackTreeViewItem(track, "HP"));
		
// 		track = _CurveEditor.AddCurve(manager.Damage, Color.red);
// 		track.Changed += sum.CurveChange; 
// 		_CurveTreeView.Controls.Add(new CurveTrackTreeViewItem(track, "Damage"));
		
// 		track = _CurveEditor.AddCurve(manager.Price, Color.blue);
// 		track.Changed += sum.CurveChange;
// 		_CurveTreeView.Controls.Add(new CurveTrackTreeViewItem(track, "Price"));
		
// 		track = _CurveEditor.AddCurve(manager.Reward, Color.yellow);
// 		track.Changed += sum.CurveChange;
// 		_CurveTreeView.Controls.Add(new CurveTrackTreeViewItem(track, "TotalGold"));
// 	}
	
// 	void OnGUI()
// 	{
// 		_Frame.OnGUI();
// 	}
	
// 	[UnityEditor.MenuItem("Tool/Tool")]
// 	static void Init()
// 	{
// 		Instance.Show();
// 	}
	
// 	public GraphEditor()
// 	{
// 		hideFlags = HideFlags.DontSave;

// 		titleContent =  new GUIContent("Curve Editor");
// 		base.position = new Rect((Screen.width - Size.x) / 2.0f, (Screen.height - Size.y) / 2.0f, Size.x, Size.y);
// 		base.minSize = new Vector2(Size.x, Size.y);

// 	}
	
// 	void Awake()
// 	{
// 		CreateUI();
// 	}
	
// 	class CurveTrackTreeViewItem : Grid
// 	{
// 		private Skill.Editor.UI.ToggleButton _TbVisible;
// 		private Skill.Framework.UI.Label _LblName;
// 		private Skill.Editor.UI.ColorField _CFColor;

// 		public CurveTrack Track { get; private set; }
// 		public CurveEditor.EditCurveInfo Info { get; private set; }

// 		public CurveTrackTreeViewItem(CurveTrack track, string name)
// 		{
// 			this.Track = track;

// 			this.ColumnDefinitions.Add(20, GridUnitType.Pixel);
// 			this.ColumnDefinitions.Add(1, GridUnitType.Star);
// 			this.ColumnDefinitions.Add(36, GridUnitType.Pixel);

// 			_TbVisible = new Skill.Editor.UI.ToggleButton() { Column = 0, IsChecked = track.Visibility == Skill.Framework.UI.Visibility.Visible };
// 			_LblName = new Label() { Column = 1, Text = name };
// 			_CFColor = new Skill.Editor.UI.ColorField() { Column = 2, Color = track.Color };

// 			this.Controls.Add(_TbVisible);
// 			this.Controls.Add(_LblName);
// 			this.Controls.Add(_CFColor);

// 			_TbVisible.Changed += _TbVisible_Changed;
// 			_CFColor.ColorChanged += _CFColor_ColorChanged;
// 		}

// 		void _CFColor_ColorChanged(object sender, System.EventArgs e)
// 		{
// 			Track.Color = _CFColor.Color;
// 		}

// 		void _TbVisible_Changed(object sender, System.EventArgs e)
// 		{
// 			Track.Visibility = _TbVisible.IsChecked ? Skill.Framework.UI.Visibility.Visible : Skill.Framework.UI.Visibility.Hidden;
// 		}
// 	}
// }
