<%@ Page Language="C#" MasterPageFile="~/Modern.master" AutoEventWireup="true" CodeFile="HelpChat.aspx.cs" Inherits="AnyminuteHelpChat" %>
<asp:Content ID="Head" ContentPlaceHolderID="head" runat="server" />
<asp:Content ID="Body" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<section class="am-card am-help-chat" id="amHelpChat"><h1>Chat</h1>
<p>Automated help for AnyMinute. This is built-in guidance, not a live support agent. Messages stay on this page and clear when you reload.</p>
<div class="am-chat-topics" aria-label="Help topics"><button type="button" data-chat-topic="time cards">Time cards</button><button type="button" data-chat-topic="breaks">Breaks</button><button type="button" data-chat-topic="approvals">Approvals</button><button type="button" data-chat-topic="business address">Business address</button><button type="button" data-chat-topic="employees">Employees</button><button type="button" data-chat-topic="projects">Projects</button><button type="button" data-chat-topic="account">Account</button></div>
<div id="amChatMessages" class="am-chat-messages" role="log" aria-live="polite" aria-relevant="additions" tabindex="0"></div>
<label for="amChatInput">Your question</label><textarea id="amChatInput" maxlength="1000" rows="3" placeholder="For example: How do I add a paid break?"></textarea>
<p>Do not enter passwords, SINs or other sensitive information.</p>
<div class="am-chat-actions"><button type="button" id="amChatSend" class="am-button">Send</button><button type="button" id="amChatClear" class="am-button secondary">Clear chat</button><a href="Help.aspx">Help Center</a><asp:HyperLink ID="Ticket" runat="server" Visible="false" Text="Contact support by ticket" /></div>
<noscript><p>Enable JavaScript to use automated chat. The Help Center and available support ticket links work without chat.</p></noscript>
</section><script src="Scripts/AnyminuteHelpChat.js?v=20261023nav1"></script>
</asp:Content>
