
using UnityEngine;
using Vuforia;

namespace UALR_EAC
{
	
	public class MoveAlphaTrackableEventHandler
		: MonoBehaviour
		, ITrackableEventHandler
	{
		#region PRIVATE_MEMBER_VARIABLES
		
		private TrackableBehaviour m_TrackableBehaviour;
		private Vector3            m_StartPosition;
		private float              m_StartTime;
		private bool               m_Tracked;
		private Renderer[]         m_Renderers;
		
		#endregion // PRIVATE_MEMBER_VARIABLES
		
		public Transform Target;
		public Vector3   MoveOffset;
		public float     MoveTime;
		
		#region UNTIY_MONOBEHAVIOUR_METHODS
		
		void Reset()
		{
			MoveOffset = new Vector3 (0.0f, 0.1f, 0.0f);
			MoveTime = 2.0f;
		}
		
		void Start()
		{
			m_StartPosition = Target.localPosition;
			m_Renderers     = Target.gameObject.GetComponentsInChildren<Renderer> (true);

			m_TrackableBehaviour = GetComponent<TrackableBehaviour>();
			if (m_TrackableBehaviour)
			{
				m_TrackableBehaviour.RegisterTrackableEventHandler(this);
			}
		}
		
		void Update()
		{
			if (!m_Tracked)
				return;
			
			float   t = (Time.time - m_StartTime) / MoveTime;
			Vector3 p = Vector3.Lerp (m_StartPosition, m_StartPosition + MoveOffset, t);
			
			Target.localPosition = p;

			setAlpha (t * t * t);
		}
		
		#endregion // UNTIY_MONOBEHAVIOUR_METHODS
		
		
		
		#region PUBLIC_METHODS
		
		/// <summary>
		/// Implementation of the ITrackableEventHandler function called when the
		/// tracking state changes.
		/// </summary>
		public void OnTrackableStateChanged(
			TrackableBehaviour.Status previousStatus,
			TrackableBehaviour.Status newStatus)
		{
			if (newStatus == TrackableBehaviour.Status.DETECTED ||
			    newStatus == TrackableBehaviour.Status.TRACKED ||
			    newStatus == TrackableBehaviour.Status.EXTENDED_TRACKED)
			{
				OnTrackingFound();
			}
			else
			{
				OnTrackingLost();
			}
		}
		
		#endregion // PUBLIC_METHODS
		
		
		
		#region PRIVATE_METHODS
		
		
		private void OnTrackingFound()
		{
			Renderer[] rendererComponents = GetComponentsInChildren<Renderer>(true);
			Collider[] colliderComponents = GetComponentsInChildren<Collider>(true);
			
			// Enable rendering:
			foreach (Renderer component in rendererComponents)
			{
				component.enabled = true;
			}
			
			// Enable colliders:
			foreach (Collider component in colliderComponents)
			{
				component.enabled = true;
			}
			
			// transition from untracked to tracked
			if (!m_Tracked) {
				m_StartTime = Time.time;
			}
			
			m_Tracked = true;
		}
		
		
		private void OnTrackingLost()
		{
			Renderer[] rendererComponents = GetComponentsInChildren<Renderer>(true);
			Collider[] colliderComponents = GetComponentsInChildren<Collider>(true);
			
			// Disable rendering:
			foreach (Renderer component in rendererComponents)
			{
				component.enabled = false;
			}
			
			// Disable colliders:
			foreach (Collider component in colliderComponents)
			{
				component.enabled = false;
			}
			
			m_Tracked = false;
			setAlpha (0.0f);
		}

		private void setAlpha(float alpha)
		{
			foreach (Renderer r in m_Renderers) {
				Material m = r.sharedMaterial;
				Color    c = m.GetColor ("_TintColor");
				c.a = Mathf.Clamp01(alpha);
				m.SetColor("_TintColor", c);
				r.sharedMaterial = m;
			}
		}

		#endregion // PRIVATE_METHODS
	}
}
