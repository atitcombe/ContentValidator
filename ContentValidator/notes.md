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