16/6
To connect to azure you need to have your credentials. vs credential don't work as well so you have to use azure cli login
To login now, you have to d this  az login --tenant 5a674a67-17a1-4b70-b1ae-33f4313a55ec  format. this is because of new security measures to select the tenant
And then for cosmos, it doesn't immediately give crud access you have to apply it via the terminal. follow this: https://learn.microsoft.com/en-us/azure/cosmos-db/nosql/how-to-grant-data-plane-access?tabs=built-in-definition%2Ccsharp&pivots=azure-interface-cli
And then it showed missing id when I had an Id. just be careful with uppercasing in everything from the models to the schema

17/6
Got this error when working with secrets
Caller is not authorized to perform action on resource. If role assignments, deny assignments or role definitions were changed recently, please observe propagation time. 
Caller: appid=3686488a-04fc-4d8a-b967-61f98ec41efe;oid=1958dd7b-bf3e-4c6f-b0f9-732cff7b1ef3;iss=https://sts.windows.net/5a674a67-17a1-4b70-b1ae-33f4313a55ec/ 
Action: 'Microsoft.KeyVault/vaults/secrets/setSecret/action'
Resource: '/subscriptions/f297cc49-e0fc-4b3e-a00c-8c61cc7a9ec4/resourcegroups/content-validator/providers/microsoft.keyvault/vaults/content-keys/secrets/cdb-name' 
Assignment: (not found) DenyAssignmentId: null DecisionReason: null Vault: content-keys;location=eastus
Fix: had to go to the access policies and create one and add my account. it doesn't automatically add you even if you created it.