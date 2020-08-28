<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class WebCamServer
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(WebCamServer))
        Me.PictureBoxREMOTO = New System.Windows.Forms.PictureBox()
        Me.Timer1 = New System.Windows.Forms.Timer(Me.components)
        CType(Me.PictureBoxREMOTO, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'PictureBoxREMOTO
        '
        Me.PictureBoxREMOTO.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.PictureBoxREMOTO.BackColor = System.Drawing.Color.Gray
        Me.PictureBoxREMOTO.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom
        Me.PictureBoxREMOTO.Location = New System.Drawing.Point(11, 11)
        Me.PictureBoxREMOTO.Margin = New System.Windows.Forms.Padding(2)
        Me.PictureBoxREMOTO.Name = "PictureBoxREMOTO"
        Me.PictureBoxREMOTO.Size = New System.Drawing.Size(667, 475)
        Me.PictureBoxREMOTO.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom
        Me.PictureBoxREMOTO.TabIndex = 126
        Me.PictureBoxREMOTO.TabStop = False
        '
        'Timer1
        '
        '
        'WebCamServer
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(689, 497)
        Me.Controls.Add(Me.PictureBoxREMOTO)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "WebCamServer"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Video EQUISDE"
        CType(Me.PictureBoxREMOTO, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents PictureBoxREMOTO As System.Windows.Forms.PictureBox
    Friend WithEvents Timer1 As System.Windows.Forms.Timer
End Class
