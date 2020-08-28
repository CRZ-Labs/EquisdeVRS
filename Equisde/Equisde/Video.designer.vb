<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class WebCamClient
    Inherits System.Windows.Forms.Form

    'Form reemplaza a Dispose para limpiar la lista de componentes.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Requerido por el Diseñador de Windows Forms
    Private components As System.ComponentModel.IContainer

    'NOTA: el Diseñador de Windows Forms necesita el siguiente procedimiento
    'Se puede modificar usando el Diseñador de Windows Forms.  
    'No lo modifique con el editor de código.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(WebCamClient))
        Me.Timer1 = New System.Windows.Forms.Timer(Me.components)
        Me.PictureBoxVISOR = New System.Windows.Forms.PictureBox()
        CType(Me.PictureBoxVISOR, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'Timer1
        '
        '
        'PictureBoxVISOR
        '
        Me.PictureBoxVISOR.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.PictureBoxVISOR.BackColor = System.Drawing.Color.DimGray
        Me.PictureBoxVISOR.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom
        Me.PictureBoxVISOR.Location = New System.Drawing.Point(11, 11)
        Me.PictureBoxVISOR.Margin = New System.Windows.Forms.Padding(2)
        Me.PictureBoxVISOR.Name = "PictureBoxVISOR"
        Me.PictureBoxVISOR.Size = New System.Drawing.Size(107, 98)
        Me.PictureBoxVISOR.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom
        Me.PictureBoxVISOR.TabIndex = 131
        Me.PictureBoxVISOR.TabStop = False
        '
        'WebCamClient
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(129, 120)
        Me.ControlBox = False
        Me.Controls.Add(Me.PictureBoxVISOR)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "WebCamClient"
        Me.Opacity = 0.5R
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Video EQUISDE"
        Me.WindowState = System.Windows.Forms.FormWindowState.Minimized
        CType(Me.PictureBoxVISOR, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents Timer1 As System.Windows.Forms.Timer
    Friend WithEvents PictureBoxVISOR As PictureBox
End Class
