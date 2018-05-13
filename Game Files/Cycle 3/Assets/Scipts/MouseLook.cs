using System;
using UnityEngine;

namespace Characters
{
    [Serializable]
    public class MouseLook
    {
        public float XSensitivity = 2f;
        public float YSensitivity = 2f;
        public bool clampRotation = true;
        public float MinimumX = -90F;
        public float MaximumX = 90F;
        public float MinimumY = -90F;
        public float MaximumY = 90F;
        public float MinimumZ = -90F;
        public float MaximumZ = 90F;
        public bool lockCursor = true;


        private Quaternion m_CharacterTargetRot;
        private Quaternion m_CameraTargetRot;
        private bool m_cursorIsLocked = true;

        public void Init(Transform character, Transform camera)
        {
            m_CharacterTargetRot = character.localRotation;
            m_CameraTargetRot = camera.localRotation;
        }


        public void LookRotation(Transform character, Transform camera)
        {
            float yRot = Input.GetAxis("Mouse X") * XSensitivity;
            float xRot = Input.GetAxis("Mouse Y") * YSensitivity;

            m_CharacterTargetRot *= Quaternion.Euler (-xRot, yRot, 0f);

            if(clampRotation)
                m_CharacterTargetRot = ClampRotationAroundAxis (m_CharacterTargetRot);

            character.localRotation = m_CharacterTargetRot;

            UpdateCursorLock();
        }

        public void SetCursorLock(bool value)
        {
            lockCursor = value;
            if(!lockCursor)
            {//we force unlock the cursor if the user disable the cursor locking helper
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }

        public void UpdateCursorLock()
        {
            //if the user set "lockCursor" we check & properly lock the cursos
            if (lockCursor)
                InternalLockUpdate();
        }

        private void InternalLockUpdate()
        {
            if(Input.GetKeyUp(KeyCode.Escape))
            {
                m_cursorIsLocked = false;
            }
            else if(Input.GetMouseButtonUp(0))
            {
                m_cursorIsLocked = true;
            }

            if (m_cursorIsLocked)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else if (!m_cursorIsLocked)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }

        Quaternion ClampRotationAroundAxis(Quaternion q) {
            q.x /= q.w;
            q.y /= q.w;
            q.z /= q.w;
            q.w = 1.0f;

            float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.x);
            float angleY = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.y);

            angleX = Mathf.Clamp(angleX, MinimumX, MaximumX);
            angleY = Mathf.Clamp(angleY, MinimumY, MaximumY);

            q.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);
            q.y = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleY);
            q.z = 0f;

            return q;
        }

    }
}
