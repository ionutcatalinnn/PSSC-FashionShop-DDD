 FashionShop - Distributed E-Commerce System Sistem distribuit pentru un magazin de fashion, implementat folosind arhitectura Domain-Driven Design și procesare asincronă în Cloud.**


 Echipa

Boghian Cătălin-Ionuț

Orza Daniel

Truț-Jorj Caius


 Descriere Domeniu

**FashionShop** este un backend scalabil pentru un retailer de îmbrăcăminte. Sistemul gestionează ciclul complet al unei comenzi, de la selecția produselor până la livrare, punând accent pe **decuplarea componentelor** prin mesagerie asincronă.

### Principii de Arhitectură
Proiectul respectă standardele moderne de dezvoltare software:
* **Principiile SOLID** și **Clean Architecture**.
* **Domain-Driven Design (DDD):** Modelarea stărilor comenzii prin tipuri imuabile (`record`) și tranziții explicite.
* **Asynchronous Messaging:** Utilizarea **Azure Service Bus** pentru a muta procesarea grea (plăți, livrare) în background.
* **Background Workers:** Servicii specializate (`IHostedService`) care procesează cozi de mesaje.

---

 Bounded Contexts Identificate

Sistemul este împărțit logic în contexte delimitate:

| Context | Responsabilitate |
| :--- | :--- |
| **🛍️ Order Context** | Preluarea comenzilor brute, validarea existenței produselor, calculul totalului (Business Logic) și generarea ID-ului unic. |
| **💳 Payment Context** | Procesarea tranzacțiilor financiare. Funcționează ca un microserviciu consumator care ascultă evenimente din Azure. |
| **🚚 Shipping Context** | Gestionarea logisticii. Generează AWB-uri și confirmă livrarea doar după validarea plății. |

---

 Event Storming & Flow

Fluxul principal este **Event-Driven**. API-ul nu blochează clientul, ci returnează `202 Accepted` imediat ce comanda intră în coadă.
Domain Events
`OrderPlacedEvent` - Comanda a fost validată, calculată și trimisă spre procesare.
`PaymentProcessedEvent` - (Intern) Worker-ul de plăți confirmă succesul.
`OrderShippedEvent` - (Intern) Worker-ul de shipping finalizează procesul.

 Fluxul de Date (Pipeline)

    A[Client API Request] -->|Validate & Calculate| B(Azure Service Bus)
    B -->|Async Message| C[Payment Worker]
    C -->|Success Logic| D[Shipping Worker]
    D -->|Persist| E[(SQL Server Database)]


(Flow: HTTP Request -> Validation -> Calculation -> Azure Queue -> Background Processing -> DB Persistence)


🛠️ Implementare DDD
1. Entity States (State Machine)
   Am modelat comanda ca o mașină de stări pentru a garanta corectitudinea datelor în fiecare pas.

UnvalidatedOrder: Datele brute (JSON) primite de la client.

ValidatedOrder: Produsele sunt verificate (cantitate > 0).

CalculatedOrder: S-a aplicat logica de preț (Preț Unit x Cantitate = Total Linie).

PlacedOrder: Comanda finală, care are un ID unic (GUID) și Timestamp, gata de procesare.

2. Operations (Transform Pattern)
   Logica nu este în Controller, ci în clase specializate de operații:

ValidateOrderOperation: Transformă Unvalidated -> Validated.

CalculateOrderOperation: Transformă Validated -> Calculated (Aplică prețuri).

PlaceOrderFinalOperation: Transformă Calculated -> Placed (Generează Identity).

3. Infrastructură Cloud
   Azure Service Bus: Coadă de mesaje (Queue) pentru fiabilitate. Dacă baza de date pică, mesajele rămân în Azure și nu se pierd.

SQL Server: Stocarea persistentă a comenzilor și a istoricului de procesare.


FashionShop_Hub/
├── FashionShop.sln
├── FashionShop.Domain/           #  Core Business Logic
│   ├── Models/
│   │   ├── Entities/             # State Records (Unvalidated, Placed...)
│   │   └── Events/               # Domain Events
│   ├── Operations/               # Clasele de transformare (Validate, Calculate)
│   ├── Workflows/                # Orchestratorul fluxului
│   └── Repositories/             # Interfețe
├── FashionShop.Data/             #  Infrastructure
│   ├── Repositories/             # Implementare OrderRepository (EF Core)
│   └── Models/                   # DTO-uri pentru Baza de Date
├── FashionShop.Events.ServiceBus/# Azure Integration
│   └── AzureServiceBusEventBus.cs # Publisher pentru Cloud
└── FashionShop_Hub/              #  API & Workers
├── Controllers/              # Endpoints
├── BackgroundServices/       # Payment & Shipping Workers
└── Program.cs                # Dependency Injection

Rulare și Configurare
Cerințe
.NET 8 SDK

SQL Server LocalDB

Cont Azure (pentru Service Bus Connection String)

API Endpoints

Metodă,Endpoint,Descriere
POST    /api/async-orders   Principalul Endpoint. Procesează comanda asincron prin Azure.
POST    /api/demo/run-complete-cycle    Simulare sincronă (pentru debug).
POST    /api/payments   Testare manuală procesare plată.
POST    /api/shipping   Testare manuală livrare (cere adresă).


Exemple de Utilizare (Testare)
1. Plasare Comandă Asincronă (Scenariul Principal)
   Trimite acest JSON către POST /api/async-orders.

{
"lines": [
{
"productCode": "TRICOU-VARA",
"quantity": 2
},
{
"productCode": "BLUGI-DENIM",
"quantity": 1
}
],
"customerName": "Student PSSC",
"address": "Campus Universitar"
}



2. Testare Manuală Shipping
   Pentru endpoint-ul POST /api/shipping:

{
"orderId": "COPIAZA-GUID-DIN-BAZA-DE-DATE",
"address": "Strada Libertatii 1",
"city": "Timisoara"
}
