using Il2Cpp;
using MelonLoader;
using UnityEngine;


namespace SillyMod
{
    [RegisterTypeInIl2Cpp]
    public class SillyCollisionDetection : MonoBehaviour
    {
        public SillyCollisionDetection(IntPtr intPtr) : base(intPtr) { }

        void OnCollisionEnter(Collision collision)
        {
            MelonLogger.Msg("Collision of " + collision.contacts[0].thisCollider.name + " with " + collision.gameObject.name);
            float collisionVelocity = collision.relativeVelocity.magnitude;
            if (collisionVelocity < 1f) return;

            ContactPoint contact = collision.contacts[0];

            SillyClass.line.SetPosition(0, contact.point + collision.relativeVelocity / 5);
            SillyClass.line.SetPosition(1, contact.point);

            float distanceToPlayer = Vector3.Distance(contact.point + collision.relativeVelocity, contact.thisCollider.ClosestPointOnBounds(contact.point + collision.relativeVelocity));
            float distanceToObject = Vector3.Distance(contact.point + collision.relativeVelocity, contact.otherCollider.ClosestPointOnBounds(contact.point + collision.relativeVelocity));

            MelonLogger.Msg(ConsoleColor.Yellow, "distanceToPlayer " + distanceToPlayer);
            MelonLogger.Msg(ConsoleColor.Yellow, "distanceToObject " + distanceToObject);

            if (distanceToObject > distanceToPlayer)
            {
                MelonLogger.Msg(ConsoleColor.Red, "Hit BY: " + collision.gameObject.name + ", force: " + (collisionVelocity));
                HUDMessage.AddMessage("Hit BY " + collision.gameObject.name + ", force: " + collision.relativeVelocity.magnitude, false, true);

                GameManager.GetPlayerManagerComponent().OnLandFromFall(collisionVelocity, 0.001f);

                if (Settings.options.dangerMode)
                {
                    if (collisionVelocity > 5f) 
                    {
                        float playerScale = Utility.AverageVector(SillyClass.worldView.localScale);
                        float objectScale = Utility.AverageVector(collision.transform.localScale);
                        float minDamage = 1f;
                        float maxDamage = 15f / playerScale;
                        float damage = Mathf.Round(collisionVelocity / 5f * objectScale) / playerScale;
                        float damageClamped = Mathf.Clamp(damage, minDamage, maxDamage);
                        //MelonLogger.Msg(damage +" "+ damageClamped);
                        if (GameManager.GetConditionComponent().m_CurrentHP - damageClamped < 15f) return;

                        GameManager.GetConditionComponent().AddHealth(-damageClamped, DamageSource.WillPower);
                    }
                }
            }
            else
            { 
                MelonLogger.Msg(ConsoleColor.Cyan, "Hit TO: " + collision.gameObject.name + ", force: " + (collisionVelocity));
                HUDMessage.AddMessage("Hit TO " + collision.gameObject.name + ", force: " + collision.relativeVelocity.magnitude, false, true);
            }
        }
    }
}
