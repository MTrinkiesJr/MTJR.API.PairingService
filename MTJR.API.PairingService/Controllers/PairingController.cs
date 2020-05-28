using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MTJR.API.PairingService.Extensions;
using MTJR.API.PairingService.Handler;
using MTJR.API.PairingService.Model;
using MTJR.API.PairingService.Model.Messages;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MTJR.API.PairingService.Controllers
{
    [ApiController]
    [Authorize]
    [Route("[controller]")]
    public class PairingController : ControllerBase
    {
        private readonly ILogger<PairingController> _logger;
        private List<PairingSession> _sessions;
        public PairingController(ILogger<PairingController> logger, List<PairingSession> sessions)
        {
            _logger = logger;
            _sessions = sessions;
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiCallResponse), 200)]
        [ProducesResponseType(typeof(bool), 202)]
        [ProducesResponseType(typeof(Session), 201)]
        [ProducesResponseType(typeof(string), 400)]
        public IActionResult Handle([FromQuery] string pairingId, [FromQuery]   [JsonConverter(typeof(StringEnumConverter))]HandshakeResourceType resource, [FromBody]string data)
        {
            (string, PairingSession) pairingSession;

            switch (resource)
            {
                case HandshakeResourceType.None:
                    return BadRequest("no resource given");
                case HandshakeResourceType.ServerHello:
                    pairingSession = IsValidPairingSession(pairingId);

                    if (pairingSession.Item2 == null)
                    {
                        pairingSession.Item1 = "OK";
                        pairingSession.Item2 = new PairingSession(pairingId);
                        _sessions.Add(pairingSession.Item2);
                    }

                    if (pairingSession.Item2.Step != HandshakeResourceType.None)
                    {
                        return BadRequest("the pairing session has not reached a valid state to do that");
                    }


                    if (pairingSession.Item1 == "OK")
                    {
                        var returnData = pairingSession.Item2.GenerateServerHello(data);
                        var apiCallResponse = new ApiCallResponse(ApiCallMethod.POST, Constants.Step1Url, JsonConvert.SerializeObject(returnData));
                        return Ok(apiCallResponse);
                    }
                    else
                    {
                        return BadRequest(pairingSession.Item1);
                    }
                case HandshakeResourceType.ClientHello:
                    pairingSession = IsValidPairingSession(pairingId, HandshakeResourceType.ClientHello);

                    if (pairingSession.Item1 == "OK")
                    {
                        var returnAckData = pairingSession.Item2.ParseClientHello(data);
                        return Accepted(returnAckData);
                    }
                    else
                    {
                        return BadRequest(pairingSession.Item1);
                    }
                case HandshakeResourceType.ServerAck:
                    pairingSession = IsValidPairingSession(pairingId, HandshakeResourceType.ServerAck);

                    if (pairingSession.Item1 == "OK")
                    {
                        var ackData = pairingSession.Item2.GenerateServerAck();
                        var ackCallResponse = new ApiCallResponse(ApiCallMethod.POST, Constants.Step2Url, JsonConvert.SerializeObject(ackData));
                        return Ok(ackCallResponse);
                    }
                    else
                    {
                        return BadRequest(pairingSession.Item1);
                    }
                case HandshakeResourceType.ClientAck:
                    pairingSession = IsValidPairingSession(pairingId, HandshakeResourceType.ClientAck);

                    if (pairingSession.Item1 == "OK")
                    {
                        var returnAckData = pairingSession.Item2.ParseClientAck(data);
                        return Accepted(returnAckData);
                    }
                    else
                    {
                        return BadRequest(pairingSession.Item1);
                    }
                case HandshakeResourceType.Session:
                    pairingSession = IsValidPairingSession(pairingId, HandshakeResourceType.Session);

                    if (pairingSession.Item1 == "OK")
                    {
                        var returnSessionData = pairingSession.Item2.GetKey();
                        return Accepted(new KeyWrapper(Convert.ToBase64String(returnSessionData)));
                    }
                    else
                    {
                        return BadRequest(pairingSession.Item1);
                    }
                default:
                    return BadRequest("no resource given");
            }
        }

        [HttpGet("data/encrypted")]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public IActionResult Data([FromQuery] string pairingId, [FromQuery] [JsonConverter(typeof(StringEnumConverter))] EncryptedResourceType resource)
        {
            var pairingSession = IsValidPairingSession(pairingId, HandshakeResourceType.Session);

            if (pairingSession.Item1 != "OK")
            {
                return BadRequest(pairingSession.Item1);
            }

            string message = "5::/com.samsung.companion:";
            switch (resource)
            {
                case EncryptedResourceType.RegisterRemoteControl:
                    message += new EventMessage(EventMessageName.registerPush, new RegisterRemoteControlMessage(), pairingSession.Item2.SecurityProvider).Serialize();
                    break;
                case EncryptedResourceType.RegisterSecondTvMessage:
                    message += new EventMessage(EventMessageName.registerPush, new RegisterSecondTvMessage(), pairingSession.Item2.SecurityProvider).Serialize();
                    break;
                case EncryptedResourceType.GetDuidMessage:
                    message += new EventMessage(EventMessageName.registerPush, new GetDuidMessage(), pairingSession.Item2.SecurityProvider).Serialize();
                    break;
                default:
                    return BadRequest("no resource given");

            }
            return Ok(message);
        }

        [HttpGet("data/encrypted/button")]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 404)]
        public IActionResult Button([FromQuery] string pairingId, [FromQuery] string buttonId)
        {
            var pairingSession = IsValidPairingSession(pairingId, HandshakeResourceType.Session);

            if (pairingSession.Item1 == "OK")
            {
                if (string.IsNullOrEmpty(pairingSession.Item2.Duid))
                {
                    return NotFound(
                        $"to call a button you get the encrypted message to get the duid from: {Constants.DuidUrl}. Then send that data via the websocket to the tv and decrypt the responnse on: {Constants.DecryptUrl}. That will store the TV identifier (DUID) in the session");
                }

                var message = "5::/com.samsung.companion:" + new EventMessage(EventMessageName.callCommon, new ButtonMessage(buttonId, pairingSession.Item2.Duid), pairingSession.Item2.SecurityProvider)
                    .Serialize();
                return Ok(message);
            }

            return BadRequest(pairingSession.Item1);
        }

        [HttpPost("data/decrypt")]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(typeof(string), 400)]

        public IActionResult Decrypt([FromQuery] string pairingId, [FromBody] string data)
        {
            var pairingSession = IsValidPairingSession(pairingId, HandshakeResourceType.Session);

            if (pairingSession.Item1 == "OK")
            {
                return Ok(pairingSession.Item2.Decrypt(data));
            }

            return BadRequest(pairingSession.Item1);
        }


        private (string, PairingSession) IsValidPairingSession(string pairingId, [JsonConverter(typeof(StringEnumConverter))]HandshakeResourceType handshakeResourceType = HandshakeResourceType.None)
        {
            var pairingSession = _sessions.FirstOrDefault(a => a.Id.Equals(pairingId, StringComparison.InvariantCultureIgnoreCase));

            if (pairingSession == null)
            {
                return ("pairing session not found", null);
            }
            else if (handshakeResourceType != HandshakeResourceType.None)
            {
                if (pairingSession.Step != handshakeResourceType)
                {
                    return ("the pairing session has not reached a valid state to do that", pairingSession);
                }
                else
                {
                    return ("OK", pairingSession);
                }
            }

            return ("OK", pairingSession);
        }
    }
}
