// author: alex@rozgo.com
// license: have fun

using System.Collections;
using System.Collections.Generic;

namespace Unity.Coroutine {
	
	// mystical Unity class
	public class YieldInstruction {
		
		internal IEnumerator routine;
		
		internal YieldInstruction () {
		}
		
		internal bool MoveNext () {
			
			var yieldInstruction = routine.Current as YieldInstruction;
			
			if (yieldInstruction != null) {
				if (yieldInstruction.MoveNext()) {
					return true;
				}
				else if (routine.MoveNext()) {
					return true;
				}
				else {
					return false;
				}
			}
			else if (routine.MoveNext()) {
				return true;
			}
			else {
				return false;
			}
		}
	}
	
	// only used as a wrapper
	public class Coroutine : YieldInstruction {
		internal Coroutine (IEnumerator routine) {
			this.routine = routine;
		}
	}
	
	// use this as a template for functions like WaitForSeconds()
	public class WaitForCount : YieldInstruction {
		
		int count = 0;
		
		public WaitForCount (int count) {
			this.count = count;
			this.routine = Count();
		}
		
		IEnumerator Count () {
			while (--count >= 0) {
				System.Console.WriteLine(count);
				yield return true;
			}
		}
	}
	
	// use this as the base class for enabled coroutines
	public class Yielder {
		
		internal List<Coroutine> coroutines = new List<Coroutine>();
		
		// just like Unity's MonoBehaviour.StartCoroutine
		public Coroutine StartCoroutine (IEnumerator routine) {
			var coroutine = new Coroutine(routine);
			coroutine.routine.MoveNext();
			coroutines.Add(coroutine);
			return coroutine;
		}
		
		// call this every frame
		protected void ProcessCoroutines () {
			for (int i=0; i<coroutines.Count; ) {
				var coroutine = coroutines[i];
				if (coroutine.MoveNext()) {
					++i;
				}
				else if (coroutines.Count > 1) {
					coroutines[i] = coroutines[coroutines.Count - 1];
					coroutines.RemoveAt(coroutines.Count - 1);
				}
				else {
					coroutines.Clear();
					break;
				}
			}
		}
	}
	
}