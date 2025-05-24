# Relay

Relay is a lightweight Entity Component System (ECS) C# framework that provides a simple way to manage entities and their behaviors. It should probably be called an Entity Component Event framework instead, since components contain both data and logic so there are no systems per se. It's a farily naive implementation, but hopefully a good starting point for implementing specific needs.

Based on [Brian Bucklew's talk about ECS and data-driven engines](https://www.youtube.com/watch?v=U03XXzcThGU).

## Features

- Entity management
- Component-based architecture
- Event system for communication between components and entities
- Tag system for quick entity lookup
- Group management for organizing entities
- Simple and intuitive API

## Installation

Add the Relay source files directly into your project.

### Dependencies

- [High Speed Priority Queue for C#](https://github.com/BlueRaja/High-Speed-Priority-Queue-for-C-Sharp)

## Example

A firesword that can deal damage to a target, where the damage is modified depending on which kind of damage components the firesword has.

```csharp
var world = new World();
var firesword = world.CreateEntity();
firesword.AddComponent(new Damage { Type = "Physical", Amount = 5 });
firesword.AddComponent(new FireDamage { Amount = 10 });

class Damage : Component
{
    public override string Id => "Damage";
    public override int Priority => 10;

    public string Type { get; set; }
    public int Amount { get; set; }

    public Damage()
    {
        StartListening("MeleeAttack");
    }

    public override void HandleEvent(Event ev)
    {
        if (ev.Type == "MeleeAttack")
        {
            // send a deal damage event to the components on this entity
            var dealDamageEvent = new Event(type: "DealDamage", target: Owner);
            dealDamageEvent["Type"] = Type;
            dealDamageEvent["Amount"] = Amount;
            dealDamageEvent["Target"] = ev.Target;
            Owner.FireEvent(dealDamageEvent);
        }
    }
}

class FireDamage : Component
{
    public override string Id => "FireDamage";
    public override int Priority => 100;

    public int Amount { get; set; }

    public FireDamage()
    {
        StartListening("DealDamage");
    }

    public override void HandleEvent(Event ev)
    {
        if (ev.Type == "DealDamage")
        {
            // modify damage
            ev.Data["Type"] += ", Fire";
            ev.Data["Amount"] = (int)ev.Data["Amount"] + Amount;
        }
    }
}

class DealDamage : Component
{
    public override string Id => "DealDamage";
    public override int Priority => 1000;

    public DealDamage()
    {
        StartListening("DealDamage");
    }

    public override void HandleEvent(Event ev)
    {
        if (ev.Type == "DealDamage")
        {
            // after all damage modifications are done, trigger TakeDamage event
            var takeDamageEvent = new Event("TakeDamage");
            takeDamageEvent.Target = (Entity)ev["Target"];
            takeDamageEvent["Type"] = ev["Type"];
            takeDamageEvent["Amount"] = ev["Amount"];
            Owner.FireEvent(takeDamageEvent);
        }
    }
}

var target = world.CreateEntity();
target.AddComponent(new Health { Value = 100 });

class Health : Component
{
    public override string Id => "Health";
    public override int Priority => 1000;

    public int Value { get; set; }

    public DealDamage()
    {
        StartListening("TakeDamage");
    }

    public override void HandleEvent(Event ev)
    {
        if (ev.Type == "TakeDamage")
        {
            string damageType = (string)ev["Type"];
            int damageAmount = (int)ev["Amount"];
            Value -= damageAmount;
        }
    }
}

var meleeAttackEvent = new Event(type: "MeleeAttack", target: firesword);
meleeAttackEvent.Data["Target"] = target;
firesword.FireEvent(meleeAttackEvent);

world.Update();
```

## Core Concepts

### Entities

Entities are basically just a container for components. They can have a tag and belong to groups.

### Components

Components contain both data and logic, where the logic is mainly how to deal with events.

### Events

Events are used for communication between components and entities. They can be fired and handled by entities, and the events are propagated through all components that are listening for the specific event types.

### Tags

Tags provide a way to quickly find entities by a string identifier.

### Groups

Groups allow you to organize entities into collections for easier management and querying.

### World

The World ties everything together. It manages the creation, destruction, tagging and grouping of entities; and firing and handling of events.

## License

This project is licensed under the MIT License - see the LICENSE file for details.
