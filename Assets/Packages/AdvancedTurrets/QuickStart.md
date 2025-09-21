
Welcome to **AdvancedTurrets**! 

This guide will cover the basics of AdvancedTurrets. I will be making a video series going into detail with further examples.


- [Getting Started](#getting-started)
- [Main Concepts](#main-concepts)
  - [UnityEvents](#unityevents)
  - [Relays](#relays)
  - [ColliderGroups](#collidergroups)
  - [Pooling](#pooling)
  - [Gizmos](#gizmos)
  - [Trajectories](#trajectories)
  - [Mathematics](#mathematics)
  - [Kinematics](#kinematics)
  - [Serialization](#serialization)
    - [AdvancedNullable\<T\>](#advancednullablet)
    - [AdvancedOptional\<T\>](#advancedoptionalt)
    - [LazyComponent(s)\<T\>](#lazycomponentst)
  - [Time Management](#time-management)
- [Support \& Contact](#support--contact)

---
# Getting Started

1. **Import the AdvancedTurrets package** into your Unity project.
2. **Attach AdvancedTurrets components** to your GameObjects..
   1. Add `Muzzles` to where your turret will fire from
   2. Add a `MuzzleLoader` to control cycle times and `Muzzles`.
   3. Add a `TargetingModule` to specify the targeting parameters.
   4. Add a `KinematicTurret` or `BeamTurret` to bring it all to life.
3. **Use the Inspector** to configure events, prefabs, turrets.

There are a a lot of examples found under **AdvancedTurrets/Examples/**. These are the recommended way for learning AdvancedTurrets features.

Most importantly - have fun and I hope you will be inspired to create something incredible. 

# Main Concepts

This section will contain the main concepts and design philosophies of AdvancedTurrets.

## UnityEvents
AdvancedTurrets leverages **UnityEvents** to enhance customization within the editor and at runtime **without requiring extensive coding expertise**.

All critical events on AdvancedTurrets objects will expose UnityEvents with args pertaining to what is happening. AdvancedTurets has native methods that accept these args and can be hooked up as-is in the inspector.

## Relays
AdvancedTurrets uses Relays (e.g., `EnableRelay`, `DisableRelay`) that **broadcast Unity callbacks** that would otherwise be tied to MonoBehaviour methods.

This **decouples logic**, making it more flexible and modular. Instead of scripting everything manually and redundantly, **simply add a relay and hook it up in the inspector**.

## ColliderGroups
Rigidbody-based turrets require Physics collision handling. AdvancedTurrets provides an event driven solution to handle these through the **`StaticColliderGroup`** and **`DynamicColliderGroup`**.

These components enable native Physics collision toggling **without scripting**, either through editor settings or **runtime events**.

## Pooling
For **enhanced performance** AdvancedTurrets includes an **object pooling system** that supports both **Prefab and instance pooling**.

## Gizmos
AdvancedTurrets includes a variety of **gizmos for debugging and visualization**. 

These are included on core objects and will **only be compiled within the Unity Editor**. These will not be compiled when building for release.

## Trajectories
The **`Trajectory`** object hides an incredible amount of complexity and is used for predicting, displaying, and analayzing kinematic motion as well as calculating trajectory interceptions. 

Once you have a `Trajectory` instance, **you have complete control over pixel-perfect kinematics and trajectory interceptions**. You can create this object yourself through the constructor or you can get one from an object that implements the **`IKinematic`** interface.

## Mathematics
Includes an **`Advanced Mathematics`** library featuring **static, deterministic, and closed-form solutions for** (among other things):
- **Quartic polynomials (ax⁴ + bx³ + cx² + dx + e)**
- **Cubic polynomials (ax³ + bx² + cx + d)**
- **Quadraic polynomials (ax^2 + bx + c)**
- **Offset Position/Quaternion rotations with constraints**

```
if (you_love_math) {
    Go check it out - The source code is included and is available statically.
}
else {
    You’re in luck, because you will never have to look at these ever!
}
```

Shoutout to **René Descartes** for paving the way.

## Kinematics
Includes the **`AdvancedKinematics`** library for **simulating and visualizing Kinematic motion**. This is **available statically** for your convenience. 

Supports **DeltaTime, FixedDeltaTime, and Instantiated** kinematic analysis.

## Serialization
AdvancedTurrets uses native serialization and provides some structs to cover common coding patterns.

### AdvancedNullable\<T>
A serializable struct that supports **nullable** struct values, filling a gap in Unity’s serialization system.

### AdvancedOptional\<T>
A serializable struct most similar to Java’s `Optional<T>` type, allowing references to **exist or not** in a controlled, predictable manner.

### LazyComponent(s)\<T>
Typically references are either **serialized in the editor** or **cached in Awake()** at runtime. AdvancedTurrets `LazyComponent<T>` captures both of these use cases in the same object.

Rapidly build and prototype by using these lazy component references and then lock them in for production when you are satisfied with the results.

- **Speed up prototyping** by using lazy component evaluation.
- **Optionally serialize** component references for production.

## Time Management
- **Supports both `Time.deltaTime` and `Time.fixedDeltaTime`** for deterministic physics.
- **`AdvancedTime`** – A wrapper for `UnityEngine.Time`, simplifies **deltaTime** access with **`AdvancedTime.SmartDeltaTime`**.

---

# Support & Contact

I’d love to hear from you! For questions, feedback, or feature requests:
- **Email:** imchrissharp@gmail.com
- **Discord:** [AdvancedTurrets](https://discord.gg/pwbMkGaaNX)
