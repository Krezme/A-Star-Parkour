using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;


namespace Krezme {
    public static class QualityOfLife {
        /// <summary>
        /// DeepClone allows the data of Objects to be duplicated without linking the objects
        /// </summary>
        /// <param name="obj"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T DeepClone<T>(this T obj) {
            using (MemoryStream stream = new MemoryStream()) {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, obj);
                stream.Position = 0;
    
                return (T) formatter.Deserialize(stream);
            }
        }

        public static GameObject FindGameObjectInChildrenWithTag(Transform parent, string tag) {
            if (parent.childCount > 0) {
                for (int i = parent.childCount-1; i >= 0; i--) {
                    GameObject gO = FindGameObjectInChildrenWithTag(parent.GetChild(i), tag);
                    if (gO != null) {
                        return gO;
                    }
                }
                if (CompareTags(parent.tag, tag)) {
                    return parent.gameObject;
                }
            }
            else {
                if (CompareTags(parent.tag, tag)) {
                    return parent.gameObject;
                }
            }
            return null;
        }

        public static bool CompareTags(string firstTag, string secondTag) {
            return firstTag == secondTag ? true : false;
        }

        //function for UI sprite to look at the player
        public static void LookAtPlayer(Transform spriteTransform, Transform playerTransform) {
            spriteTransform.LookAt(playerTransform);
        }

        /// <summary>
        /// Generates all possible random numbers without repeating 
        /// </summary>
        /// <param name="min">The minimum generated number (including)</param>
        /// <param name="max">The maximum generated number (including)</param>
        /// <param name="amount">Amount of numbers you want to generate. Entering a greater amount than the numbers between "min" and "max" will not repeat numbers.</param>
        /// <returns>Integer array of the random numbers without repetition</returns>
        public static int[] RandomNumberArrayWithoutRepeating(int min, int max) {
            List<int> numbers = new List<int>();
            for (int i = min; i <= max; i++) {
                numbers.Add(i);
            }
            int [] randomNumbers = new int[numbers.Count];
            int amount = numbers.Count;
            for (int i = 0; i < amount; i++) {
                int randomIndex = UnityEngine.Random.Range(0, numbers.Count);
                randomNumbers[i] = numbers[randomIndex];
                numbers.RemoveAt(randomIndex);
            }
            return randomNumbers;
        }

        /// <summary>
        /// Generate an amount of random numbers without repeating
        /// </summary>
        /// <param name="min">The minimum generated number (including)</param>
        /// <param name="max">The maximum generated number (including)</param>
        /// <param name="amount">Amount of numbers you want to generate. Entering a greater amount than the numbers between "min" and "max" will not repeat numbers.</param>
        /// <returns>Integer array of the random numbers without repetition</returns>
        public static int[] RandomNumberArrayWithoutRepeating(int min, int max, int amount) {
            int [] randomNumbers = new int[amount];
            List<int> numbers = new List<int>();
            for (int i = min; i <= max; i++) {
                numbers.Add(i);
            }
            if (numbers.Count < amount) {
                amount = numbers.Count;
            }
            for (int i = 0; i < amount; i++) {
                int randomIndex = UnityEngine.Random.Range(0, numbers.Count);
                randomNumbers[i] = numbers[randomIndex];
                numbers.RemoveAt(randomIndex);
            }
            return randomNumbers;
        }

        /// <summary>
        /// Generates all possible random numbers without repeating 
        /// </summary>
        /// <param name="min">The minimum generated number (including)</param>
        /// <param name="max">The maximum generated number (including)</param>
        /// <param name="amount">Amount of numbers you want to generate. Entering a greater amount than the numbers between "min" and "max" will not repeat numbers.</param>
        /// <returns>Integer list of the random numbers without repetition</returns>
        public static List<int> RandomNumberListWithoutRepeating(int min, int max) {
            List<int> randomNumbers = new List<int>();
            List<int> numbers = new List<int>();
            for (int i = min; i <= max; i++) {
                numbers.Add(i);
            }
            int amount = numbers.Count;
            for (int i = 0; i < amount; i++) {
                int randomIndex = UnityEngine.Random.Range(0, numbers.Count);
                randomNumbers.Add(numbers[randomIndex]);
                numbers.RemoveAt(randomIndex);
            }
            return randomNumbers;
        }

        /// <summary>
        /// Generate an amount of random numbers without repeating
        /// </summary>
        /// <param name="min">The minimum generated number (including)</param>
        /// <param name="max">The maximum generated number (including)</param>
        /// <param name="amount">Amount of numbers you want to generate. Entering a greater amount than the numbers between "min" and "max" will not repeat numbers.</param>
        /// <returns>Integer list of the random numbers without repetition</returns>
        public static List<int> RandomNumberListWithoutRepeating(int min, int max, int amount) {
            List<int> randomNumbers = new List<int>();
            List<int> numbers = new List<int>();
            for (int i = min; i <= max; i++) {
                numbers.Add(i);
            }
            if (numbers.Count < amount) {
                amount = numbers.Count;
            }
            for (int i = 0; i < amount; i++) {
                int randomIndex = UnityEngine.Random.Range(0, numbers.Count);
                randomNumbers.Add(numbers[randomIndex]);
                numbers.RemoveAt(randomIndex);
            }
            return randomNumbers;
        }

        /// <summary>
        /// Generates a list of the fibonacci sequence up to a certain amount
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        public static List<int> FibonacciSequenceList(int amount) {
            if (amount >= 1) {
                List<int> fibonacciSequence = new List<int>() {
                    0,
                    1
                };
                for (int i = 0; i < amount-2; i++) {
                    fibonacciSequence.Add(fibonacciSequence[i] + fibonacciSequence[i + 1]);
                }
                return fibonacciSequence;
            }
            else {
                return new List<int>() {0};
            }
        }

        /// <summary>
        /// Generates the number of the fibonacci sequence at a certain index
        /// </summary>
        /// <param name="requestedNumber">the sequence number requested</param>
        /// <returns>A the specified number in the sequence</returns>
        public static int FibonacciSequenceInt(int requestedNumber) {
            if (requestedNumber >= 1) {
                List<int> fibonacciSequence = new List<int>() {
                    0,
                    1
                };
                for (int i = 0; i < requestedNumber-2; i++) {
                    fibonacciSequence.Add(fibonacciSequence[i] + fibonacciSequence[i + 1]);
                }
                return fibonacciSequence[requestedNumber-1];
            }
            else {
                return 0;
            }
        }

        #region Arcing a Projectile

        public static float GetInitialSpeedToArc(float angle, float gravity, Vector3 projectilePosition, Vector3 targetPosition) {
            float distance = Vector3.Distance(new Vector3(targetPosition.x, 0, targetPosition.z), new Vector3(projectilePosition.x, 0, projectilePosition.z));

            float verticalDistance = projectilePosition.y - targetPosition.y;

            float radians = GetRadians(angle);
            //Debug.Log(Mathf.Sqrt(Mathf.Abs((0.5f * Mathf.Abs(gravity) * Mathf.Pow(distance, 2)) / (distance * Mathf.Tan(radians) + verticalDistance))));
            //Debug.Log(Mathf.Sqrt((0.5f * Mathf.Abs(gravity) * Mathf.Pow(distance, 2)) / (distance * Mathf.Tan(radians) + verticalDistance)));

            return (1 / Mathf.Cos(radians)) * Mathf.Sqrt(Mathf.Abs((0.5f * Mathf.Abs(gravity) * Mathf.Pow(distance, 2)) / (distance * Mathf.Tan(radians) + verticalDistance)));
        }

        public static float GetRadians(float angle) {
            return angle * Mathf.Deg2Rad;
        }

        public static Vector3 GetVector3VelocityToArc(float initialSpeed, float radians) {
            return new Vector3(0, initialSpeed * Mathf.Sin(radians), initialSpeed * Mathf.Cos(radians));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="normalizedDirection"></param>
        /// <param name="origin"></param>
        /// <param name="target"></param>
        /// <param name="projectileForward"></param>
        /// <returns></returns>
        public static float GetAngleBetweenObjects(Vector3 normalizedDirection, Vector3 origin, Vector3 target, Vector3 projectileForward) {

            float angle = Vector3.Angle(normalizedDirection, new Vector3(target.x, 0, target.z) - new Vector3(origin.x, 0, origin.z));

            //!LEARN WHY THIS WORKS AND WHAT DOT AND CROSS DO 
            if (target.y >= origin.y) {
                if (Vector3.Dot(Vector3.Cross(normalizedDirection, new Vector3(target.x, 0, target.z) - new Vector3(origin.x, 0, origin.z)), projectileForward) < 0) {
                    angle = 360 - angle;
                }
            }
            else {
                if (Vector3.Dot(Vector3.Cross(normalizedDirection, new Vector3(target.x, 0, target.z) - new Vector3(origin.x, 0, origin.z)), projectileForward) > 0) {
                    angle = 360 - angle;
                }
            }
            
            return angle;
        }

        public static float GetTimeToReachDestination(float speed, float distance) {
            float timeToReachDestination = distance / speed;
            //float timeToReachDestinationWithGravity = Mathf.Sqrt((2 * distance) / speed);
        
            return timeToReachDestination;
        }

        public static float GetFreeFallingDistance(float time) {
            float freeFallingDistance = 0.5f * 9.81f * Mathf.Pow(time, 2);
            //float freeFallingDistance = speed * time + 0.5f * -9.81f * Mathf.Pow(time, 2);
            return freeFallingDistance;
        }

        #endregion

        /// <summary>
        /// Calculating the camera's centre point
        /// </summary>
        /// <param name="camera">The camera used for the calculation</param>
        /// <returns>Vector2 of the camera's centre point</returns>
        public static Vector2 GetCameraCentrePoint(Camera camera){
            return new Vector2(Screen.width / 2f, Screen.height / 2f);
        }

        /// <summary>
        /// Calculating the camera's centre point and converting it to a Ray
        /// </summary>
        /// <param name="camera">The camera used for the calculation</param>
        /// <returns>Ray from the centre of the camera</returns>
        public static Ray GetCameraCentrePointAsRay(Camera camera){
            Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
            return camera.ScreenPointToRay(screenCenterPoint);
        }
    }

    public enum Bool3D {
        Null,
        False,
        True
    }
}
