using System.Collections.Generic;
using System;

// Types

public enum EasingStyle {	
	Linear,
	Constant,
	Sine,
	Quad,
	Cubic,
	Quartic,
	Quintic,
	Expo,
	Circular,
	Back,
	Elastic,
	Bounce,
}

public enum EasingDirection {
	In,
	Out,
	InOut,
	OutIn,
}

// Script

public class Tween {
	// Instance Properties
	
	public Info Instructions;
	public object Object;
	public bool Playing;
	public float StartTime;
	
	public Dictionary<string, object> Properties;
	public Dictionary<string, object> LerptimeProperties;

	// Static Variables
	
	static List<Tween> TweenInstances = new List<Tween>();
	
	public static float Tick {
		get {
			DateTime epoch = new DateTime(1970, 1, 1);
			DateTime now = DateTime.Now;
			return (float) ((now - epoch).TotalMilliseconds);
		}
		set {throw new Exception("Cannot set value.");}
	}

	// Constructor
	
	public Tween(object GameObject, Info info) {
		this.Instructions = info;
		this.Object = GameObject;
		this.Properties = new Dictionary<string, object>() {};
		Tween.TweenInstances.Add(this);
	}

	// Instance Methods

	public Tween Add(string property, object val) {
		this.Properties[property] = val;
		return this;
	}

	public Tween Play() {
		this.LerptimeProperties = new Dictionary<string, object>(this.Properties);
		this.StartTime = Tween.Tick;
		return this;
	}

	// Overrides

	public override string ToString() {
		if (this.Properties.Count >= 1) {
			string truncate = this.Properties.Count > 1 ? "..." : "";
			
			foreach (KeyValuePair<string, object> pair in this.Properties) {
				return $"Tween ({pair.Key} = {pair.Value}{truncate})";
			}
		}
		return "Tween (Empty)";
	}
	
	// Easing Instructions

	public static float GetValue(double lerp, EasingStyle style, EasingDirection? direction) {
		float x = (float) lerp;
		
		float pi = (float) Math.PI;
		
		var cos = Math.Cos;
		var sin = Math.Sin;
		var pow = Math.Pow;
		var sqrt = Math.Sqrt;
		
		// Main Function
		
		float f(float x) {
			float result = 0;
			if (x <= 0) {return 0;}
			if (x >= 1) {return 1;}
			switch (style) {
				default:
					result = x;
					break;
				
				case EasingStyle.Constant:
					result = 0f;
					break;
				
				case EasingStyle.Sine:
					result = (float) (1 - cos((x * pi) / 2));
					break;
				
				case EasingStyle.Quad:
					result = x * x;
					break;
				
				case EasingStyle.Cubic:
					result = x * x * x;
					break;
				
				case EasingStyle.Quartic:
					result = x * x * x * x;
					break;
				
				case EasingStyle.Quintic:
					result = x * x * x * x * x;
					break;
				
				case EasingStyle.Expo:
					result = (float) pow(2, 10 * x - 10);
					break;
				
				case EasingStyle.Circular:
					result = (float) (1 - sqrt(1 - pow(x, 2)));
					break;
				
				case EasingStyle.Back:
					float overshoot = 1.85f;
					float level = overshoot + 1;
					result = level * x * x * x - overshoot * x * x;
					break;
				
				case EasingStyle.Elastic:
					result = (float) (-pow(2, 10 * x - 10) * sin((x * 10 - 10.75) * (2 * pi) / 3));
					break;
				
				case EasingStyle.Bounce:
					float n1 = 7.5625f;
					float d1 = 2.75f;
					
					if (x < 1 / d1) {
					    result = n1 * x * x;
					} else if (x < 2 / d1) {
					    result = n1 * (x - 1.5f / d1) * x + 0.75f;
					} else if (x < 2.5 / d1) {
					    result = n1 * (x - 2.25f / d1) * x + 0.9375f;
					} else {
					    result = n1 * (x - 2.625f / d1) * x + 0.984375f;
					}
					break;
			}
			return result;
		}

		switch (direction) {
			default:
				return f(x);
			case EasingDirection.Out:
				return 1 - f(1 - x);
			case EasingDirection.InOut:
				if (x <= 0.5) {
					return f(2 * x) / 2;
				} else {
					return (1 - f(1 - x * 2 + 1) + 1) / 2;
				}
			case EasingDirection.OutIn:
				if (x <= 0.5) {
					return (1 - f(1 - 2 * x)) / 2;
				} else {
					return f(2 * x - 1) / 2 + 0.5f;
				}
		}
	}
	
	// TweenInfo

	public class Info {
		public float Lifetime;
		public EasingStyle Style;
		public EasingDirection Direction;
		public int Repeats;
		public bool UndoRepeats;
		public float RepeatDelay;
		
		public Info(float time = 0.5f, EasingStyle style = EasingStyle.Linear, EasingDirection direction = EasingDirection.Out, int repeats = 0, bool undoRepeats = false, float repeatDelay = 0f) {
			Lifetime = time;
			Style = style;
			Direction = direction;
			Repeats = repeats;
			UndoRepeats = undoRepeats;
			RepeatDelay = repeatDelay;
		}

		public override string ToString() {
			return $"Tween.Info({Lifetime}, EasingStyle.{Style}, EasingDirection.{Direction}, {Repeats}, {UndoRepeats.ToString().ToLower()}, {RepeatDelay})";
		}  
	}
	
	// Unity Events

	void Update() {
		
	}
	
	// Entry Point
	
	public static void Main(string[] args) {
		Tween test = new Tween(Tween, new Tween.Info());
	}
}