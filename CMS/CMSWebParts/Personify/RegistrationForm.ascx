<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSWebParts_Membership_Registration_RegistrationForm"
    CodeFile="~/CMSWebParts/Personify/RegistrationForm.ascx.cs" %>
<%@ Register Src="~/CMSFormControls/Captcha/SecurityCode.ascx" TagName="SecurityCode" TagPrefix="uc1" %>
<asp:Label ID="lblError" runat="server" EnableViewState="false" Visible="false" />
<asp:Label ID="lblInfo" runat="server" EnableViewState="false" Visible="false" />
<asp:Panel ID="pnlRegForm" runat="server" DefaultButton="btnRegister">
<style type="text/css">
    .InfoLabel {
        visibility: hidden;
        display: none;
    }
	.registration_form {padding:0 0 50px 0;
	}
	.registration_form input[type="text"], .registration_form input[type="name"],.registration_form input[type="email"], .registration_form input[type="password"],.registration_form textarea{width:96%;padding:0 2%;
	}
	.registration_form select {background-position:98% center;
	}
	@media only screen and (max-width:767px) {
		.article-page-content {padding:50px 5% 20px;}
		.registration_form .container{padding:0;}
		.registration_form .row {margin:0;
		}
	}

</style>
	<cms:DataForm ID="formUser" runat="server" IsLiveSite="true" DefaultFormLayout="SingleTable" />
	<asp:PlaceHolder runat="server" ID="plcCaptcha">
		<div class="form-horizontal">
			<div class="form-group">
				<div class="editing-form-label-cell">
					<cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblCaptcha" ResourceString="webparts_membership_registrationform.captcha" />
				</div>
				<div class="editing-form-value-cell">
					<uc1:SecurityCode ID="captchaElem" runat="server" />
				</div>
			</div>
		</div>
	</asp:PlaceHolder>

    <div class="submit btn-realistic" style="float: right !important;">
	    <asp:LinkButton ID="btnRegister" runat="server"></asp:LinkButton>
    </div>
</asp:Panel>

<script type="text/javascript">
	$(document).ready(function () {

		$('.confirmctrl label').css('display', 'block');

		$('.RegisterButton').click(function () {
			var valid = 0;
			var valAddr = 0;
			$('.form-horizontal input[type="text"],input[type="password"],select').not('[id$=txtAddress2]').each(function () {

				if ($(this).val() == "") {
					$(this).closest('.editing-form-value-cell').children().last().css('display', 'block');
					valid = valid + 1;
				}
				else {
					$(this).closest('.editing-form-value-cell').children().last().css('display', 'none');
				}

			});

			$('.addr').find('input[type="text"],select').not('[id$=txtAddress2]').each(function () {
				if ($(this).val() == "") {
					valid = valid + 1;
					valAddr = valAddr + 1;
				}
			});
			if (valAddr > 0)
				$('.addr span').css('display', 'block');
			else
				$('.addr span').css('display', 'none');



			if ($('input:password:eq(0)').val() != "" && $('input:password:eq(1)').val() != "" && $('input:password:eq(0)').val() != $('input:password:eq(1)').val()) {
				valid = valid + 1;
				$('.pwdmatch').css('display', 'block');
			}
			else {
				$('.pwdmatch').css('display', 'none');
			}

			var pattern = /^([a-z\d!#$%&'*+\-\/=?^_`{|}~\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]+(\.[a-z\d!#$%&'*+\-\/=?^_`{|}~\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]+)*|"((([ \t]*\r\n)?[ \t]+)?([\x01-\x08\x0b\x0c\x0e-\x1f\x7f\x21\x23-\x5b\x5d-\x7e\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]|\\[\x01-\x09\x0b\x0c\x0d-\x7f\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]))*(([ \t]*\r\n)?[ \t]+)?")@(([a-z\d\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]|[a-z\d\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF][a-z\d\-._~\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]*[a-z\d\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])\.)+([a-z\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]|[a-z\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF][a-z\d\-._~\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]*[a-z\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])\.?$/i;
			if (!pattern.test($('.mail').val())) {
				valid = valid + 1;
				$('.erremail').css('display', 'block');
			}
			else {
				$('.erremail').css('display', 'none');
			}

			if (valid == 0)
				return true;
			else
				return false;

		});


	});
</script>