using System.Collections.Generic;
using System;
using globals;

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
	//Bounce,
}

public enum EasingDirection {
	In,
	Out,
	InOut,
	OutIn,
}

// Script

public class GameObject { // Testing
	public double Transparency;
	public double Size;
	public double Position;
	public double Orientation;
	public string Name;
	public readonly int InstanceId = 0;
	private static int ObjectId = 0;
	
	public GameObject() {
		this.Transparency = 0;
		this.Size = 1;
		this.Position = 0;
		this.Orientation = 0;
		this.Name = "Object";
		this.InstanceId = GameObject.ObjectId;
		GameObject.ObjectId += 1;
	}
}

public class Tween {
	// Instance Properties
	
	public readonly Info Instructions;
	public readonly GameObject Object;
	public bool Playing;
	
	private double StartTime;
	
	Dictionary<string, object> Properties;
	Dictionary<string, object> LerptimeProperties;

	// Static Variables
	
	private static List<Tween> TweenInstances = new List<Tween>();

	private static List<Type> Lerpables = new List<Type>() {
		typeof(int), typeof(float), typeof(double), typeof(long), typeof(decimal)
	};

	private static List<Type> NumberTypes = new List<Type>() {
		typeof(int), typeof(float), typeof(double), typeof(long), typeof(decimal)
	};

	// Static Methods
	
	public static bool IsField(GameObject obj, string name) {
		if (obj.GetType().GetField(name) != null) {
			return true;
		} else {
			return false;
		}
	}

	public static Type GetTypeOfField(GameObject obj, string name) {
		return obj.GetType().GetField(name).GetValue(obj).GetType();
	}

	public static bool IsReadOnly(GameObject obj, string name) {
		return obj.GetType().GetField(name).IsInitOnly;
	}
	
	public static bool IsNumberType(Type t) {
		return NumberTypes.Contains(t);
	}
	
	public static bool IsLerpable(GameObject obj, string name) {
		if (Tween.IsField(obj, name)) {
			return Tween.Lerpables.Contains(Tween.GetTypeOfField(obj, name));
		} else {
			return false;
		}
	}
	
	// Constructor
	
	public Tween(GameObject obj, Info info) {
		this.Instructions = info;
		this.Object = obj;
		this.Properties = new Dictionary<string, object>() {};
		Tween.TweenInstances.Add(this);
	}

	// Instance Methods
	
	public object this[string name] {
		get {
			return this.Properties[name];
		}
		set {
			if (Tween.IsField(this.Object, name)) { // Is this a field?
				if (!Tween.IsReadOnly(this.Object, name)) { // Is field accessible?
					if (Tween.IsLerpable(this.Object, name)) { // Is this field lerpable?
						// Is the field type equal to the setting type?
						Type proptype = Tween.GetTypeOfField(this.Object, name);
						Type valuetype = value.GetType();
						if (proptype == valuetype || (IsNumberType(proptype) && IsNumberType(valuetype))) {
							this.Properties[name] = value;
						} else {
							throw new ArgumentException($"Field '{name}' expects a '{Tween.GetTypeOfField(this.Object, name).Name.ToLower()}'. Got a '{value.GetType().Name.ToLower()}'.");
						}
					} else {
						throw new ArgumentException($"Field '{name}' is not a tweenable because it is a '{Tween.GetTypeOfField(this.Object, name).Name.ToLower()}'.");
					}
				} else {
					throw new ArgumentException($"Field '{name}' is readonly.");
				}
			} else {
				throw new KeyNotFoundException($"'{name}' is not a valid field of '{this.Object.Name}'");
			}
		}
	}
		
	public Tween Play() {
		this.LerptimeProperties = new Dictionary<string, object>(this.Properties);
		this.StartTime = os.tick;
		return this;
	}
	
	// Overrides

	public override string ToString() {
		if (this.Properties.Count >= 1) {
			string truncate = this.Properties.Count > 1 ? $", +{this.Properties.Count - 1} more" : "";
			string playing = this.Playing ? "(Playing) " : "";
			
			foreach (KeyValuePair<string, object> pair in this.Properties) {
				return $"Tween {playing}({pair.Key} = {pair.Value}{truncate})";
			}
		}
		return "Tween (Empty)";
	}
	
	// Easing Instructions

	public static float GetValue(double lerp, EasingStyle style, EasingDirection? direction) {
		float x = (float) lerp;
		
		float pi = (float) Math.PI;
		
		Func<double, double> cos = Math.Cos;
		Func<double, double> sin = Math.Sin;
		Func<double, double, double> pow = Math.Pow;
		Func<double, double> sqrt = Math.Sqrt;
		
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
				
				// case EasingStyle.Bounce:
				// 	float n1 = 7.5625f;
				// 	float d1 = 2.75f;
					
				// 	if (x < 1 / d1) {
				// 	    result = n1 * x * x;
				// 	} else if (x < 2 / d1) {
				// 	    result = n1 * (x - 1.5f / d1) * x + 0.75f;
				// 	} else if (x < 2.5 / d1) {
				// 	    result = n1 * (x - 2.25f / d1) * x + 0.9375f;
				// 	} else {
				// 	    result = n1 * (x - 2.625f / d1) * x + 0.984375f;
				// 	}
				// 	break;
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
		public double Lifetime;
		public EasingStyle Style;
		public EasingDirection Direction;
		public int Repeats;
		public bool UndoRepeats;
		public float RepeatDelay;
		
		public Info(double time = 0.5, EasingStyle style = EasingStyle.Linear, EasingDirection direction = EasingDirection.Out, int repeats = 0, bool undoRepeats = false, float repeatDelay = 0f) {
			Lifetime = time;
			Style = style;
			Direction = direction;
			Repeats = repeats;
			UndoRepeats = undoRepeats;
			RepeatDelay = repeatDelay;
		}

		// Overrides
		
		public override string ToString() {
			return $"Tween.Info({Lifetime}, EasingStyle.{Style}, EasingDirection.{Direction}, {Repeats}, {UndoRepeats.ToString().ToLower()}, {RepeatDelay})";
		}  
	}
	
	// Unity Events

	void Update() {
		
	}
	
	// Entry Point
	
	public static void Main(string[] args) {
		GameObject obj = new GameObject();
		Tween tw = new Tween(obj, new Tween.Info()) {
			["Transparency"] = 0.5,
			["Size"] = 2,
		};
		Console.WriteLine(tw);
	}
}

/*

Compile to .dll and .exe:

csc /target:library /out:main.dll main.cs
csc /target:library /out:main.exe main.cs

*/