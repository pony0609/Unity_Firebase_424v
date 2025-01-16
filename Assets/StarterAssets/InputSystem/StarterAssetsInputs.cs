using System.Collections;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace StarterAssets {
    public class StarterAssetsInputs : MonoBehaviour {
        [Header("Character Input Values")]
        public Vector2 move;
        public Vector2 look;
        public bool jump;
        public bool sprint;
        public bool DrawWeapon;
        public bool Attack;
        [Header("Movement Settings")]
        public bool analogMovement;

        [Header("Mouse Cursor Settings")]
        public bool cursorLocked = true;
        public bool cursorInputForLook = true;

#if ENABLE_INPUT_SYSTEM
        public void OnMove(InputValue value) {
            MoveInput(value.Get<Vector2>());
        }

        public void OnLook(InputValue value) {
            if (cursorInputForLook) {
                LookInput(value.Get<Vector2>());
            }
        }

        public void OnJump(InputValue value) {
            JumpInput(value.isPressed);
        }

        public void OnSprint(InputValue value) {
            SprintInput(value.isPressed);
        }

        public void OnDrawWeapon(InputValue value) // ²K¥[ OnDrawWeapon ¤èªk
{
            Debug.Log("DrawWeapon input received");
            //DrawWeaponInput(value.isPressed);
            ToggleDrawWeapon();
        }

        public void OnAttack(InputValue value) {

            Debug.Log("WeaponAttack input received");
            //DrawWeaponInput(value.isPressed);
            Attacks();
        }
#endif


        public void MoveInput(Vector2 newMoveDirection) {
            move = newMoveDirection;
        }

        public void LookInput(Vector2 newLookDirection) {
            look = newLookDirection;
        }

        public void JumpInput(bool newJumpState) {
            jump = newJumpState;
        }

        public void SprintInput(bool newSprintState) {
            sprint = newSprintState;
        }

        public void ToggleDrawWeapon() {
            DrawWeapon = !DrawWeapon;
            Debug.Log("DrawWeapon state changed to: " + DrawWeapon);
        }

        public void Attacks() {
            Attack = true;
            Debug.Log("Attacks state changed to: " + Attack);
            StartCoroutine(ResetAttackAfterDelay(0.1f));
        }

        private IEnumerator ResetAttackAfterDelay(float delay) {
            yield return new WaitForSeconds(delay);
            Attack = false;
            Debug.Log("Attacks state changed to: " + Attack);
        }

        private void OnApplicationFocus(bool hasFocus) {
            SetCursorState(cursorLocked);
        }

        private void SetCursorState(bool newState) {
            Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
        }
    }

}