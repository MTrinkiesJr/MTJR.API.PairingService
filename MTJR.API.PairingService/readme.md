# Samsung TV J+ (2014) Series Handshake API
This API handles the required handshake payloads and encryption/decryption for Samsung J series smart TVs

## API Desgin
The API is designed in `ASP .NET CORE 3.1` which also inclues an API Documentation called `SWAGGER` which can be accessed under `http://{IpOfAPI}:{PortOfAPI}/swagger`. 
### Swagger
In `SWAGGER` all API Endpoints are listed with their `StatusCodes` and `ResponseTypes`. They also could be tested from there, but you will get an `401 Unauthorized` because we cannot set headers there (see authentication)

### Authentication
The API is protected via a simple `GUID` which is configured in the `appsettings.json` which is only accessible on the server. To use that `Authentication` you always need to inlcude the Header `X-Api-Guid : {GUID}`.
The configuration looks like this: 
```
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "AllowedHosts": "*",
  "ApiGuid":  "1234" 
}

```

## Logging (implementation coming soon)

## How to use this API
To do a successfull handshake a couple of HTTP requests are required to get a valid encryption key and a session id.
The API only provides the payloads that are send to the TV.

All values in the `{}` must be replaced by the actual values.

1. request a pin page on the TV with `http://{IpOfTv}:8080/ws/apps/CloudPINPage`
2. post that pin (as body and header `Content-Type : application/json` ) to 
	`http://{IpOfAPI}:{PortOfAPI}/pairing?pairingId={UniqeIdForSession}&resource=ServerHello`
3. the reponse is like:

	    {
		    "data": "010200000000000000008A00000006363534333231...(truncated)"
	    }
	   
4. wrap the `DATA`into:
	```
	{
	    "auth_data": 
	    {
		    "auth_type" : "SPC",
		    "GeneratorServerHello" : "{DATA}"
	    }
	}
	```
5. POST the wrapped data to:
	`http://{IpOfTv}:8080/ws/pairing?step=1&app_id=com.samsung.companion&device_id={SameIdOfYourChoice}&type=1`
6. the response is like:
	```
	{
		"auth_data":"	
		{
			"auth_type" : "SPC",
			"request_id" : "0",
			"GeneratorClientHello" : "010100000000000000009E00000006363534333231040C1D...(truncated)"
		}
	}

7.  get the `GeneratorClientHello` and the `RequestId`with regex: `[{\"\w:]GeneratorClientHello[\\\":]*([\d\w]*)` and `[{\"\w:]request_id[\\\":]*([\d\w]*)`
8. send the `GeneratorClientHello`to:
	`http://{IpOfApi}:{PortOfApi}/pairing?pairingId={UniqeIdForSession}&resource=ClientHello`
	
	this will internally validate against the previous data (true or false)
9. send a POST request (without any body) to:
	`http://{IpOfApi}:{PortOfApi}/pairing?pairingId={UniqeIdForSession}&resource=ServerAck`
10. the reponse is like:
	```
	{
		"data" : "01030000000000000000144D4ECE8F61EACE69A95C38D028B...(truncated)"
	}
	```
11. wrap the `DATA` in:
	```
	{
		"auth_data":
		{
			"auth_type" : "SPC",
			"request_id" : "{RequestId}",
			"ServerAckMsg" : "{DATA}"
		}
	}
	```
12. POST the wrapped data to:
	`http://{IpOfTv}:8080/ws/pairing?step=2&app_id=com.samsung.companion&device_id={SameIdOfYourChoice}&type=1`
13. the response is like:
	```
	{
		"auth_data":"
		{
			"auth_type" : "SPC",
			"request_id" : "0",
			"ClientAckMsg" : "0104000000000000000014BEC4282F6456E06F7090...{truncated}",
			"session_id" : "21"
		}
	}
	```
14. grab the `ClientAckMsg` with `[{\"\w:]ClientAckMsg[\\\":]*([\d\w]*)` and the `SessionId` with `[{\"\w:]session_id[\\\":]*([\d\w]*)`
15. send the  `ClientAckMsg` (as body with header `Content-Type : application/json`) to:
	`http://{IpOfApi}:{PortOfApi}/pairing?pairingId={UniqeIdForSession}&resource=ClientAck` 
	
	this will internally validate against the previous data (true or false)
16. send a POST request (without any body) to:
	`http://{IpOfApi}:{PortOfApi}/pairing?pairingId={UniqeIdForSession}&resource=Session`
17. the response is like:
	```
	{
		"Key" : "SgJSLL5L0Pjh34...(truncated)"
	}
	```
18. close the pin page on TV with DELETE request to:
	`http://{IpOfTv}:8080/ws/apps/CloudPINPage/run`

Save the `SessionId` and the `Key` for later.
Now you're successfully paired to the TV. You can use the same `PIN` to always request a `EncryptionKey` just start at step `2.` so the same process without showing a PIN page on TV ;-)

From now you can use that `KEY` and the `SessionId` to encrypt/decrypt the required messages which are end trough the websocket connection, **OR** use the encryption/decryption endpoints from the API which uses the created `KEY`and `SessionId` automatically.

## Encryption/Decryption (implemented, docs comming soon)