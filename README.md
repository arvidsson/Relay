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
// setup in which order we process behaviours
world.BehaviourCategoryOrder = new() { "Damage", "Health" };

var firesword = world.CreateEntity();
firesword.AddBehaviours(
    new DamageBehaviour { Type = "Physical", Amount = 5 },
    new FireDamageBehaviour { Type = "Fire", Amount = 10 }
);

class DamageBehaviour : Behaviour
{
    public string Type { get; set; }
    public int Amount { get; set; }

    public DamageBehaviour()
    {
        Category = "Damage";
        Priority = 10;
        ListenTo("MeleeAttack");
    }

    public override BehaviourResult HandleEvent(Event ev)
    {
        if (ev.Type == "MeleeAttack")
        {
            // send a deal damage event to the behaviours on this entity
            var dealDamageEvent = new Event(type: "DealDamage", target: Owner);
            dealDamageEvent["Type"] = Type;
            dealDamageEvent["Amount"] = Amount;
            dealDamageEvent["Target"] = ev.Target;
            Owner.FireEvent(dealDamageEvent);
        }
        return BehaviourResult.Continue;
    }
}

class FireDamageBehaviour : Behaviour
{
    public string Type { get; set; }
    public int Amount { get; set; }

    public FireDamageBehaviour()
    {
        Category = "Damage";
        Priority = 20;
        ListenTo("DealDamage");
    }

    public override BehaviourResult HandleEvent(Event ev)
    {
        if (ev.Type == "DealDamage")
        {
            // modify damage
            ev.Data["Type"] += (", " + Type);
            ev.Data["Amount"] = (int)ev.Data["Amount"] + Amount;
        }
        return BehaviourResult.Continue;
    }
}

class DealDamageBehaviour : Behaviour
{
    public DealDamageBehaviour()
    {
        Category = "Damage";
        Priority = 100;
        ListenTo("DealDamage");
    }

    public override BehaviourResult HandleEvent(Event ev)
    {
        if (ev.Type == "DealDamage")
        {
            // after all damage modifications are done, trigger TakeDamage event
            var takeDamageEvent = new Event(type: "TakeDamage", target: (Entity)ev["Target"]);
            takeDamageEvent["Type"] = ev["Type"];
            takeDamageEvent["Amount"] = ev["Amount"];
            Owner.FireEvent(takeDamageEvent);
        }
        return BehaviourResult.Continue;
    }
}

var monster = world.CreateEntity();
monster.AddBehaviours(new HealthBehaviour { Value = 100 });

class HealthBehaviour : Behaviour
{
    public int Value { get; set; }

    public HealthBehaviour()
    {
        Category = "Health";
        Priority = 10;
        ListenTo("TakeDamage");
    }

    public override BehaviourResult HandleEvent(Event ev)
    {
        if (ev.Type == "TakeDamage")
        {
            string damageType = (string)ev["Type"];
            int damageAmount = (int)ev["Amount"];
            Value -= damageAmount;
        }
        return BehaviourResult.Continue;
    }
}

var meleeAttackEvent = new Event(type: "MeleeAttack", target: firesword);
meleeAttackEvent.Data["Target"] = monster;
firesword.FireEvent(meleeAttackEvent);

world.Update();
```

## Core Concepts

### Entities

Entities are basically just a container for components and behaviours. They can have a tag and belong to groups.

### Components

Components are pure data.

### Behaviours

Behaviours are mostly just logic, that handles events. A Behaviour belongs to a Category and has a Priority. The behaviours process events in the order of the category and then in the priority order within that category. Set exact category order in the World's BehaviourCategoryOrder.

### Events

Events are used for communication between behaviours and entities. They can be fired and handled by entities, and the events are propagated through all behaviours that are listening for the specific event types.

### Tags

Tags provide a way to quickly find entities by a string identifier.

### Groups

Groups allow you to organize entities into collections for easier management and querying.

### World

The World ties everything together. It manages the creation, destruction, tagging and grouping of entities; and firing and handling of events.

## License

This project is licensed under the MIT License - see the LICENSE file for details.
