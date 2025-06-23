16/6
To connect to azure you need to have your credentials. vs credential don't work as well so you have to use azure cli login
To login now, you have to d this  az login --tenant 5a674a67-17a1-4b70-b1ae-33f4313a55ec  format. this is because of new security measures to select the tenant
And then for cosmos, it doesn't immediately give crud access you have to apply it via the terminal. follow this: https://learn.microsoft.com/en-us/azure/cosmos-db/nosql/how-to-grant-data-plane-access?tabs=built-in-definition%2Ccsharp&pivots=azure-interface-cli
And then it showed missing id when I had an Id. just be careful with uppercasing in everything from the models to the schema

17/6
Got this error when working with secrets
Caller is not authorized to perform action on resource. If role assignments, deny assignments or role definitions were changed recently, please observe propagation time. 
Caller: appid=?????;iss=???
Action: 'Microsoft.KeyVault/vaults/secrets/setSecret/action'
Resource: '/subscriptions/f297cc49????' 
Assignment: (not found) DenyAssignmentId: null DecisionReason: null Vault: content-keys;location=eastus
Fix: had to go to the access policies and create one and add my account. it doesn't automatically add you even if you created it.

18/6
I did managed identities. I uploaded the api to app service via the portal and hosted it there. and then i created a managed identity for the resource
Facing some problems with security access.
Made sure that the role based access for the app service was `secrets user` in the key vault access
then i kep trunning into cosmos 500 error because of readmedata access not allowed even after i added everything possible.
I had to add the permissions via the cli because it's not possible via the UI.
the command was like this:

az cosmosdb sql role assignment create \
    --resource-group "<name-of-existing-resource-group>" \
    --account-name "<name-of-existing-nosql-account>" \
    --role-definition-id "<id-of-new-role-definition>" \
    --principal-id "<id-of-existing-identity>" \
    --scope "/subscriptions/aaaa0a0a-bb1b-cc

just note the prinicple id from the app service. use tjos doc: https://learn.microsoft.com/en-us/azure/cosmos-db/nosql/how-to-grant-data-plane-access?tabs=built-in-definition%2Ccsharp&pivots=azure-interface-cli
it owrked successfully did it

21/6
Then i went into functions and how to do the trigger.
So i put the function in its own project but still inside the solution.
and then i created a function app in azure. reason for this is that when the function is in its own project it can scale independently.

so there are two different identities with deploying. one for the builder(the pipeline), and one for the function app which the code in the function assumes
for the yaml pipeline it should use an app registration(from entra id) which then has a federated credential - this helps for no managing secrets
so after this, add this app registration id as a contributor and reader in the function app's 

first error: "the 'uses' attribute must be a path, a Docker image, or owner/repo@ref."
solution: the actions/setup-dotnetv4 was missing an @

then it is also good to store the secrets in the pipelines secrets. go to repo's settings -> enviroments -> secrets. i put it in env secrets but use best judgment on repo vs env secret
use this documentation to see the format of the azure login: https://github.com/azure/login?tab=readme-ov-file

22/6
second error: then i faced a read/write permission for oidc. 
solution: adding this to the deploy job
permissions:
      id-token: write # Required to request the OIDC token
      contents: read  # Required for checkout and other actions

also checkout is called first in the yaml because checkout basically copies the repo to be able to built in the git environment

third error: Failed to fetch federated token from GitHub. Please make sure to give write permissions to id-token in the workflow.
this was even after putting the permissions in the yaml
fix: by default repos only have read level permissions for workflows so the write access was being blocked. Actions -> general -> workflow permission. Change it to read and write permissions


fourth error: .azurefunctions directory not found at root level
fix: in the upload artifact for depl step add this : include-hidden-files: true. reason is because it builds on windows but not linux system because linux
automatically skips .dadada files. so you have to manually make sure it's included

also make sure your code runs. always do dotnet build and dotnet run and also dotnet publish.

then i learnt about trying to use the function in the api. 
so architecture wise how it interacts it can either do directly through a fire and forget after the api performs the post call
or we can make the api send the request to a messaging queue and then the function responds to the queue once something is there.
The fire and forget is the easiest to set up and it's good for low traffic or tasks that doesn't need separate systems and non critical tasks
The messaging queue is very scalable and it is good for tasks that must happen regardless if the function is down. the fire and forget might miss messages
if the function is down. But messaging queue is overkill if at least one of those three conditions isn't true. It's also good if the user doens't need
an immediate reply.





