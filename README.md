# LoadTestCC -- Command&Control for Distributed Load Tests
Framework for running multi-client distributed load tests.
Consists of:
- A Command & Control WebApi for orchestrating remote agents
- A library to be used in agent processes, that connects to the C&C and manages local test clients.

Based on .NET Core (ASP.NET Core)

## Running the sample

- Run the LoadTestCC server:

`cd src/LoadTestCC`
`dotnet run`

- Check its status at `http://localhost:5000/ltcc/getstatus`

- Run one or more agents

`cd sample\TestAgentExeEmpty`
`dotnet run`

- Tell the CC to use some clients:  `http://localhost:5000/ltcc/settarget/5`